using System;
using System.IO;
using System.Text;
using MAI2.Util;
using MAI2System;
using Manager;
using UnityEngine;
using UnityEngine.Profiling;

namespace MAI2
{
	public class Profiler
	{
		public const float DefaultUpdateInterval = 1000f;

		private bool update_;

		private float updateInterval_ = 1000f;

		private float usedHeap_;

		private float usedMono_;

		private float totalMono_;

		private float totalAllocatedMemory_;

		private float totalReservedMemory_;

		private int totalTextures_;

		private int totalMeshes_;

		private float usedTextureMemory_;

		private float usedMeshMemory_;

		private int totalGameObjects_;

		private int totalComponents_;

		private int totalMaterials_;

		private int totalObjects_;

		private float usedGameObjectMemory_;

		private float usedComponentMemory_;

		private float usedMaterialMemory_;

		private float usedObjectMemory_;

		private int framesPerSecond_;

		private DateTime startTime_;

		private int garbageCollect_;

		private int minFPS = 1000;

		private int maxFPS;

		private double prevTime;

		private int frameCounter;

		public float UpdateInterval
		{
			get
			{
				return updateInterval_;
			}
			set
			{
				updateInterval_ = value;
			}
		}

		public float UsedHeap => usedHeap_;

		public float UsedMono => usedMono_;

		public float TotalMono => totalMono_;

		public float TotalAllocatedMemory => totalAllocatedMemory_;

		public float TotalReservedMemory => totalReservedMemory_;

		public int TotalTextures => totalTextures_;

		public int TotalMeshes => totalMeshes_;

		public float UsedTextureMemory => usedTextureMemory_;

		public float UsedMeshMemory => usedMeshMemory_;

		public int TotalGameObjects => totalGameObjects_;

		public int TotalComponents => totalComponents_;

		public int TotalMaterials => totalMaterials_;

		public int TotalObjects => totalObjects_;

		public float UsedGameObjectMemory => usedGameObjectMemory_;

		public float UsedComponentMemory => usedComponentMemory_;

		public float UsedMaterialMemory => usedMaterialMemory_;

		public float UsedObjectMemory => usedObjectMemory_;

		public int FPS => framesPerSecond_;

		public int FPS_Min => minFPS;

		public int FPS_Max => maxFPS;

		public DateTime StartTime => startTime_;

		public int GC => garbageCollect_;

		public bool UpdateTexture { get; set; }

		public bool UpdateMesh { get; set; }

		public bool UpdateGameObject { get; set; }

		public bool UpdateComponent { get; set; }

		public bool UpdateMaterial { get; set; }

		public bool UpdateObject { get; set; }

		public bool Update
		{
			get
			{
				return update_;
			}
			set
			{
				update_ = value;
			}
		}

		public Profiler()
		{
			startTime_ = DateTime.Now;
		}

		private float toMega(uint value)
		{
			return (float)(Math.Round((double)value * 9.5367431640625E-07 * 1000.0) * 0.001);
		}

		private float toMega(long value)
		{
			return (float)(Math.Round((double)value * 9.5367431640625E-07 * 1000.0) * 0.001);
		}

		private void getObjectUsedMemory<T>(out int count, out float usedMemory) where T : UnityEngine.Object
		{
			long num = 0L;
			T[] array = Resources.FindObjectsOfTypeAll<T>();
			count = array.Length;
			T[] array2 = array;
			foreach (T o in array2)
			{
				num += UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(o);
			}
			usedMemory = toMega(num);
		}

		public void dumpObjectUsedMemory<T>(string path) where T : UnityEngine.Object
		{
			StringBuilder stringBuilder = Singleton<SystemConfig>.Instance.getStringBuilder();
			T[] array = Resources.FindObjectsOfTypeAll<T>();
			foreach (T val in array)
			{
				stringBuilder.AppendFormat("{0} {1},{2}\n", val.name, val.GetInstanceID(), UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(val));
			}
			try
			{
				File.WriteAllText(path, stringBuilder.ToString());
			}
			catch
			{
			}
		}

		public void update()
		{
			double gameMSecAddD = GameManager.GetGameMSecAddD();
			if ((double)minFPS > gameMSecAddD || minFPS == 0)
			{
				minFPS = (int)gameMSecAddD;
			}
			if ((double)maxFPS < gameMSecAddD)
			{
				maxFPS = (int)gameMSecAddD;
			}
			frameCounter++;
			prevTime += gameMSecAddD;
			if (update_)
			{
				if (prevTime >= (double)updateInterval_)
				{
					updateMemoryUse();
				}
				garbageCollect_ = System.GC.CollectionCount(0);
			}
		}

		public void updateMemoryUse()
		{
			framesPerSecond_ = frameCounter;
			frameCounter = 0;
			prevTime = 0.0;
			minFPS = 0;
			maxFPS = 0;
			usedHeap_ = toMega(UnityEngine.Profiling.Profiler.usedHeapSizeLong);
			usedMono_ = toMega(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong());
			totalMono_ = toMega(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong());
			totalAllocatedMemory_ = toMega(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
			totalReservedMemory_ = toMega(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong());
			if (UpdateTexture)
			{
				getObjectUsedMemory<Texture>(out totalTextures_, out usedTextureMemory_);
			}
			if (UpdateMesh)
			{
				getObjectUsedMemory<Mesh>(out totalMeshes_, out usedMeshMemory_);
			}
			if (UpdateGameObject)
			{
				getObjectUsedMemory<GameObject>(out totalGameObjects_, out usedGameObjectMemory_);
			}
			if (UpdateComponent)
			{
				getObjectUsedMemory<Component>(out totalComponents_, out usedComponentMemory_);
			}
			if (UpdateMaterial)
			{
				getObjectUsedMemory<Material>(out totalMaterials_, out usedMaterialMemory_);
			}
			if (UpdateObject)
			{
				getObjectUsedMemory<UnityEngine.Object>(out totalObjects_, out usedObjectMemory_);
			}
		}
	}
}
