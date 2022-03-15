using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Note;
using UnityEngine;

namespace Manager
{
	public class NotesReader
	{
		private class BarData2
		{
			public BarData barData = new BarData();

			public MeterChangeData meterData = new MeterChangeData();
		}

		private readonly int[,] _mirrorPos = new int[4, 8]
		{
			{ 0, 1, 2, 3, 4, 5, 6, 7 },
			{ 7, 6, 5, 4, 3, 2, 1, 0 },
			{ 3, 2, 1, 0, 7, 6, 5, 4 },
			{ 4, 5, 6, 7, 0, 1, 2, 3 }
		};

		private readonly int[,] _mirrorPosTouchE = new int[4, 8]
		{
			{ 0, 1, 2, 3, 4, 5, 6, 7 },
			{ 0, 7, 6, 5, 4, 3, 2, 1 },
			{ 4, 3, 2, 1, 0, 7, 6, 5 },
			{ 4, 5, 6, 7, 0, 1, 2, 3 }
		};

		private readonly SlideType[,] mirrorSlide = new SlideType[4, 14]
		{
			{
				SlideType.Slide_None,
				SlideType.Slide_Straight,
				SlideType.Slide_Circle_L,
				SlideType.Slide_Circle_R,
				SlideType.Slide_Curve_L,
				SlideType.Slide_Curve_R,
				SlideType.Slide_Thunder_L,
				SlideType.Slide_Thunder_R,
				SlideType.Slide_Corner,
				SlideType.Slide_Bend_L,
				SlideType.Slide_Bend_R,
				SlideType.Slide_Skip_L,
				SlideType.Slide_Skip_R,
				SlideType.Slide_Fan
			},
			{
				SlideType.Slide_None,
				SlideType.Slide_Straight,
				SlideType.Slide_Circle_R,
				SlideType.Slide_Circle_L,
				SlideType.Slide_Curve_R,
				SlideType.Slide_Curve_L,
				SlideType.Slide_Thunder_R,
				SlideType.Slide_Thunder_L,
				SlideType.Slide_Corner,
				SlideType.Slide_Bend_R,
				SlideType.Slide_Bend_L,
				SlideType.Slide_Skip_R,
				SlideType.Slide_Skip_L,
				SlideType.Slide_Fan
			},
			{
				SlideType.Slide_None,
				SlideType.Slide_Straight,
				SlideType.Slide_Circle_R,
				SlideType.Slide_Circle_L,
				SlideType.Slide_Curve_R,
				SlideType.Slide_Curve_L,
				SlideType.Slide_Thunder_R,
				SlideType.Slide_Thunder_L,
				SlideType.Slide_Corner,
				SlideType.Slide_Bend_R,
				SlideType.Slide_Bend_L,
				SlideType.Slide_Skip_R,
				SlideType.Slide_Skip_L,
				SlideType.Slide_Fan
			},
			{
				SlideType.Slide_None,
				SlideType.Slide_Straight,
				SlideType.Slide_Circle_L,
				SlideType.Slide_Circle_R,
				SlideType.Slide_Curve_L,
				SlideType.Slide_Curve_R,
				SlideType.Slide_Thunder_L,
				SlideType.Slide_Thunder_R,
				SlideType.Slide_Corner,
				SlideType.Slide_Bend_L,
				SlideType.Slide_Bend_R,
				SlideType.Slide_Skip_L,
				SlideType.Slide_Skip_R,
				SlideType.Slide_Fan
			}
		};

		protected int _playerID;

		protected LoadType _loadType;

		protected NotesHeader _header = new NotesHeader();

		protected NotesComposition _composition = new NotesComposition();

		protected NotesData _note = new NotesData();

		protected NotesTotal _total = new NotesTotal();

		private BarData _barDataCleared = new BarData();

		public NotesReader()
		{
			init(0);
		}

		public void init(int playerID)
		{
			_loadType = LoadType.LOAD_INVALID;
			_header.clear();
			_composition.clear();
			_note.clear();
			_total.clear();
			_playerID = playerID;
		}

		public bool load(string fileName, LoadType loadType = LoadType.LOAD_FULL)
		{
			bool result = false;
			if (checkFormat(fileName) == FormatType.FORMAT_M2S)
			{
				result = loadM2S(fileName, loadType);
			}
			return result;
		}

		private bool loadM2S(string fileName, LoadType loadType = LoadType.LOAD_FULL)
		{
			NotesRecord notesRecord = new NotesRecord();
			if (!notesRecord.load(fileName))
			{
				return false;
			}
			bool result = loadM2SMain(fileName, notesRecord.getList(), loadType);
			_header._notesName = fileName;
			return result;
		}

		private bool loadM2SMain(string fileName, M2SRecordList records, LoadType loadType)
		{
			init(_playerID);
			_loadType = loadType;
			_header._notesName = fileName;
			int num = 1;
			foreach (M2SRecord record in records)
			{
				bool flag = false;
				M2sfileCategoryID.Def value = record.getType().getCategory().getValue();
				if (value == M2sfileCategoryID.Def.Begin || value == M2sfileCategoryID.Def.Totals)
				{
					flag = loadHeader(record);
				}
				else
				{
					flag = false;
				}
			}
			if (loadType == LoadType.LOAD_HEADER)
			{
				return true;
			}
			foreach (M2SRecord record2 in records)
			{
				M2sfileRecordID.Def value2 = record2.getType().getValue();
				if (value2 == M2sfileRecordID.Def.BPM)
				{
					BPMChangeData bPMChangeData = new BPMChangeData();
					bPMChangeData.time.init(record2.getBar(), record2.getGrid(), this);
					bPMChangeData.bpm = record2.getF32(3u);
					_composition._bpmList.Add(bPMChangeData);
				}
			}
			calcBPMList();
			int noteIndex = 0;
			int slideIndex = 0;
			foreach (M2SRecord record3 in records)
			{
				bool flag2 = false;
				switch (record3.getType().getCategory().getValue())
				{
				case M2sfileCategoryID.Def.Composition:
					flag2 = loadComposition(record3, num);
					break;
				case M2sfileCategoryID.Def.Notes:
					flag2 = loadNote(record3, num, ref noteIndex, ref slideIndex);
					break;
				case M2sfileCategoryID.Def.Totals:
					flag2 = loadTotal(record3, num);
					break;
				default:
					flag2 = false;
					break;
				case M2sfileCategoryID.Def.Begin:
					break;
				}
				if (flag2)
				{
					num++;
				}
			}
			calcNoteTiming();
			calcEach();
			calcSlide();
			calcEndTiming();
			calcBPMInfo();
			calcBarList();
			calcSoflanList();
			calcClickList();
			calcTotal();
			return true;
		}

		public NotesHeader GetHeader()
		{
			return _header;
		}

		public NotesComposition GetCompositioin()
		{
			return _composition;
		}

		public NoteDataList GetNoteList()
		{
			return _note._noteData;
		}

		public List<TouchChainList> GetTouchChainList()
		{
			return _note._touchChainList;
		}

		public NotesTotal GetTotal()
		{
			return _total;
		}

		public ClickDataList GetClickFirstList()
		{
			return _header.getClickFirstList(_composition._barList[1]);
		}

		public float GetBPM_Index(uint index)
		{
			if (index >= _composition._bpmList.Count())
			{
				return 150f;
			}
			return _composition._bpmList[(int)index].bpm;
		}

		public float GetBPM_Time(float msec)
		{
			for (int num = _composition._bpmList.Count - 1; num >= 0; num--)
			{
				BPMChangeData bPMChangeData = _composition._bpmList[num];
				if (!(bPMChangeData.time.msec > msec))
				{
					return bPMChangeData.bpm;
				}
			}
			if (_composition._bpmList.Count == 0)
			{
				return 120f;
			}
			return _composition._bpmList[0].bpm;
		}

		public float GetBPM_Frame(float frame)
		{
			return GetBPM_Time(frame * 16.666666f);
		}

		public bool GetMeter_Index(out int denomi, out int num, uint index)
		{
			if (index >= _composition._meterList.Count)
			{
				denomi = 4;
				num = 4;
				return false;
			}
			denomi = (int)_composition._meterList[(int)index].denomi;
			num = (int)_composition._meterList[(int)index].num;
			return true;
		}

		public bool GetMeter_Time(out int denomi, out int num, float msec)
		{
			if (_composition._meterList.Count == 0)
			{
				denomi = 4;
				num = 4;
				return false;
			}
			foreach (MeterChangeData item in Enumerable.Reverse(_composition._meterList))
			{
				if (!(item.time.msec > msec))
				{
					denomi = (int)item.denomi;
					num = (int)item.num;
					return true;
				}
			}
			denomi = (int)_composition._meterList[0].denomi;
			num = (int)_composition._meterList[0].num;
			return true;
		}

		public bool getMeter_Frame(out int denomi, out int num, float frame)
		{
			return GetMeter_Time(out denomi, out num, frame * 16.666666f);
		}

		public int getNoteNum()
		{
			return _note._noteData.Count();
		}

		public int getNoteIndexMax()
		{
			int num = -1;
			foreach (NoteData noteDatum in _note._noteData)
			{
				if (num < noteDatum.indexNote)
				{
					num = noteDatum.indexNote;
				}
			}
			return num;
		}

		public bool getNoteByIndex(uint index, out NoteData ans)
		{
			if (index > _note._noteData.Count())
			{
				ans = null;
				return false;
			}
			ans = _note._noteData[(int)index];
			return true;
		}

		public bool getNoteByTime(float from, float range, ref NoteDataList ans)
		{
			if (_note._noteData.Count == 0)
			{
				return false;
			}
			if (_note._noteData.Last().time.msec < from)
			{
				return false;
			}
			float num = from + range;
			if (_note._noteData.First().time.msec > num)
			{
				return false;
			}
			ans.Clear();
			foreach (NoteData noteDatum in _note._noteData)
			{
				if (noteDatum.time.msec >= from && noteDatum.time.msec <= num)
				{
					ans.Add(noteDatum);
				}
			}
			return !ans.Empty();
		}

		public int getResolution()
		{
			return _header._resolutionTime;
		}

		public void getNoteAll(ref NoteDataList ans)
		{
			ans.Clear();
			ans = _note._noteData;
		}

		public bool IsEnableTouchNotes()
		{
			return _header._touchNum != 0;
		}

		public int GetMaxNotes()
		{
			return _header._maxNotes;
		}

		public BarDataList getBarDataList(BarType type)
		{
			return _composition._barList[(int)type];
		}

		public BarData getBarData(BarType type, float msec)
		{
			BarData result = _barDataCleared;
			BarDataList barDataList = getBarDataList(type);
			if (!barDataList.Any())
			{
				return result;
			}
			for (int num = barDataList.Count() - 1; num >= 0; num--)
			{
				BarData barData = barDataList[num];
				if (!(barData.time.msec > msec))
				{
					result = barData;
					break;
				}
			}
			return result;
		}

		public BarData getBarNextData(BarType type, float msec)
		{
			BarDataList barDataList = getBarDataList(type);
			if (!barDataList.Any())
			{
				return null;
			}
			for (int i = 0; i < barDataList.Count; i++)
			{
				if (!(barDataList[i].time.msec <= msec))
				{
					return barDataList[i];
				}
			}
			return null;
		}

		public uint getBar(float msec)
		{
			return getBarData(BarType.GAMEBAR, msec).numTotal;
		}

		public uint getBeat(float msec, bool isInBar = true)
		{
			BarData barData = getBarData(BarType.GAMEBEAT, msec);
			if (!isInBar)
			{
				return barData.numTotal;
			}
			return barData.numBar;
		}

		public bool isBar(float msecNow, float msecPre)
		{
			return getBar(msecNow) > getBar(msecPre);
		}

		public bool isBeat(float msecNow, float msecPre)
		{
			return getBeat(msecNow, isInBar: false) > getBeat(msecPre, isInBar: false);
		}

		public float getBarFloat(float msec, BarType type = BarType.GAMEBAR)
		{
			BarData barData = getBarData(type, msec);
			BarData barNextData = getBarNextData(type, msec);
			float num = barData.numTotal;
			if (barNextData != null)
			{
				float msec2 = barData.time.msec;
				float msec3 = barNextData.time.msec;
				if (msec3 > msec2)
				{
					num += (msec - msec2) / (msec3 - msec2);
				}
			}
			return num;
		}

		public float getSoflanSeqTiming(float currentMsec, float targetMsec)
		{
			if (!_composition._soflanList.Any())
			{
				return targetMsec;
			}
			if (currentMsec > targetMsec)
			{
				return targetMsec;
			}
			float num = targetMsec;
			foreach (SoflanData soflan in _composition._soflanList)
			{
				if (!(targetMsec <= soflan.time.msec))
				{
					if (!(currentMsec >= soflan.end.msec))
					{
						float num2 = Math.Max(soflan.time.msec, currentMsec);
						float num3 = Math.Min(soflan.end.msec, targetMsec);
						if (!(num2 > num3))
						{
							num += (num3 - num2) * (soflan.speed - 1f);
						}
					}
					continue;
				}
				return num;
			}
			return num;
		}

		public bool isSoflanding(float currentMsec)
		{
			if (!_composition._soflanList.Any())
			{
				return false;
			}
			foreach (SoflanData soflan in _composition._soflanList)
			{
				if (currentMsec <= soflan.time.msec)
				{
					break;
				}
				if (!(currentMsec >= soflan.end.msec) && soflan.speed == 0f)
				{
					return true;
				}
			}
			return false;
		}

		public float getAnimTiming(float currentMsec)
		{
			if (!_composition._soflanList.Any())
			{
				return currentMsec;
			}
			float num = currentMsec;
			foreach (SoflanData soflan in _composition._soflanList)
			{
				if (!(currentMsec <= soflan.time.msec))
				{
					if (currentMsec >= soflan.end.msec)
					{
						num += (soflan.end.msec - soflan.time.msec) * (soflan.speed - 1f);
						continue;
					}
					return num + (currentMsec - soflan.time.msec) * (soflan.speed - 1f);
				}
				return num;
			}
			return num;
		}

		public float getSoflanChange(float currentMsec)
		{
			if (!_composition._soflanList.Any())
			{
				return 1f;
			}
			SoflanData soflanData = null;
			SoflanData soflanData2 = null;
			foreach (SoflanData soflan in _composition._soflanList)
			{
				if (currentMsec + 1f < soflan.time.msec)
				{
					soflanData2 = soflan;
					break;
				}
				if (currentMsec - 1f >= soflan.end.msec)
				{
					soflanData = soflan;
					continue;
				}
				return soflan.speed;
			}
			if (soflanData != null && soflanData2 != null)
			{
				float num = Mathf.Max(soflanData2.time.msec - soflanData.end.msec, 1f);
				float num2 = Mathf.Max(Mathf.Min((currentMsec - soflanData.end.msec) / num, 1f), 0f);
				num2 = -2f * num2 * num2 * num2 + 3f * num2 * num2;
				return NoteUtil.getRate(soflanData.speed, soflanData2.speed, num2);
			}
			return 1f;
		}

		protected bool loadHeader(M2SRecord rec)
		{
			bool result = true;
			switch (rec.getType().getValue())
			{
			case M2sfileRecordID.Def.Begin:
			{
				for (uint num = 0u; num < 2; num++)
				{
					_header._version[num].str = rec.getStr(num + 1);
					string[] array = _header._version[num].str.Split('.');
					if (array.Count() == 3)
					{
						_header._version[num].major = int.Parse(array[0]);
						_header._version[num].minor = int.Parse(array[1]);
						_header._version[num].release = int.Parse(array[2]);
					}
					else
					{
						_header._version[num].major = 0;
						_header._version[num].minor = 0;
						_header._version[num].release = 0;
					}
				}
				break;
			}
			case M2sfileRecordID.Def.CREATOR:
				_header._creator = rec.getStr(1u);
				break;
			case M2sfileRecordID.Def.BPM_DEF:
				_header._bpmInfo.firstBPM = rec.getF32(1u);
				_header._bpmInfo.defaultBPM = rec.getF32(2u);
				_header._bpmInfo.maxBPM = rec.getF32(3u);
				_header._bpmInfo.minBPM = rec.getF32(4u);
				result = false;
				break;
			case M2sfileRecordID.Def.MET_DEF:
				_header._metInfo.denomi = (uint)rec.getS32(1u);
				_header._metInfo.num = (uint)rec.getS32(2u);
				break;
			case M2sfileRecordID.Def.RESOLUTION:
				_header._resolutionTime = rec.getS32(1u);
				break;
			case M2sfileRecordID.Def.CLK_DEF:
				_header._clickFirst = rec.getS32(1u);
				break;
			case M2sfileRecordID.Def.PROGJUDGE_BPM:
				_header._progJudgeBPM = rec.getF32(1u);
				break;
			case M2sfileRecordID.Def.TUTORIAL:
				_header._isTutorial = rec.getS32(1u) > 0;
				break;
			case M2sfileRecordID.Def.FES:
				_header._isFes = rec.getS32(1u) > 0;
				break;
			case M2sfileRecordID.Def.T_REC_TTP:
			case M2sfileRecordID.Def.T_REC_THO:
				_header._touchNum += rec.getS32(1u);
				break;
			case M2sfileRecordID.Def.T_NUM_ALL:
				_header._maxNotes = rec.getS32(1u);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		protected bool loadComposition(M2SRecord rec, int index)
		{
			bool result = true;
			NotesTime time = default(NotesTime);
			time.init(rec.getBar(), rec.getGrid(), this);
			switch (rec.getType().getValue())
			{
			case M2sfileRecordID.Def.BPM:
				result = false;
				break;
			case M2sfileRecordID.Def.MET:
			{
				MeterChangeData meterChangeData = new MeterChangeData();
				meterChangeData.time = time;
				meterChangeData.denomi = (uint)rec.getS32(3u);
				meterChangeData.num = (uint)rec.getS32(4u);
				_composition._meterList.Add(meterChangeData);
				break;
			}
			case M2sfileRecordID.Def.STP:
			{
				SoflanData soflanData2 = new SoflanData();
				soflanData2.time = time;
				soflanData2.speed = 0f;
				soflanData2.end.init(rec.getBar(), rec.getGrid() + rec.getS32(3u), this);
				_composition._soflanList.Add(soflanData2);
				break;
			}
			case M2sfileRecordID.Def.SFL:
			{
				SoflanData soflanData = new SoflanData();
				soflanData.time = time;
				soflanData.speed = rec.getF32(4u);
				soflanData.end.init(rec.getBar(), rec.getGrid() + rec.getS32(3u), this);
				_composition._soflanList.Add(soflanData);
				break;
			}
			case M2sfileRecordID.Def.CLK:
			{
				ClickData clickData = new ClickData();
				clickData.time = time;
				_composition._clickList.Add(clickData);
				break;
			}
			default:
				result = false;
				break;
			}
			return result;
		}

		protected bool loadNote(M2SRecord rec, int index, ref int noteIndex, ref int slideIndex)
		{
			bool result = true;
			M2sfileNotetypeID m2sfileNotetypeID = FumenUtil.tag2Type(rec.getStr(0u));
			if (!FumenUtil.tag2Check(rec.getStr(0u)) || !m2sfileNotetypeID.getNotetype().isValid())
			{
				return false;
			}
			NoteData noteData = new NoteData();
			noteData.type = m2sfileNotetypeID.getNotetype();
			noteData.time.init(rec.getBar(), rec.getGrid(), this);
			noteData.end = noteData.time;
			noteData.startButtonPos = ConvertMirrorPosition(rec.getPos());
			noteData.index = index;
			noteData.indexNote = noteIndex;
			noteIndex++;
			switch (m2sfileNotetypeID.getNotetype().getValue())
			{
			case NoteTypeID.Def.Begin:
			case NoteTypeID.Def.Star:
			case NoteTypeID.Def.Break:
			case NoteTypeID.Def.BreakStar:
			case NoteTypeID.Def.ExTap:
			case NoteTypeID.Def.ExStar:
				_note._noteData.Add(noteData);
				break;
			case NoteTypeID.Def.TouchTap:
				noteData.touchArea = rec.getTouchArea();
				noteData.effect = rec.getTouchEffect();
				noteData.touchSize = rec.getTouchSize();
				switch (noteData.touchArea)
				{
				case NoteTypeID.TouchArea.C:
					noteData.startButtonPos = 0;
					break;
				case NoteTypeID.TouchArea.E:
					noteData.startButtonPos = ConvertMirrorTouchEPosition(rec.getPos());
					break;
				}
				_note._noteData.Add(noteData);
				break;
			case NoteTypeID.Def.Hold:
			case NoteTypeID.Def.ExHold:
			{
				int holdLen2 = rec.getHoldLen();
				noteData.end.init(rec.getBar(), rec.getGrid() + holdLen2, this);
				_note._noteData.Add(noteData);
				break;
			}
			case NoteTypeID.Def.Slide:
			{
				noteData.indexSlide = slideIndex++;
				SlideData slideData = noteData.slideData;
				int slideWaitLen = rec.getSlideWaitLen();
				int slideShootLen = rec.getSlideShootLen();
				slideData.targetNote = ConvertMirrorPosition(rec.getSlideEndPos());
				slideData.shoot.time.init(rec.getBar(), rec.getGrid() + slideWaitLen, this);
				slideData.shoot.index = noteIndex;
				slideData.arrive.time.init(rec.getBar(), rec.getGrid() + slideWaitLen + slideShootLen, this);
				slideData.arrive.index = noteIndex;
				noteData.end = slideData.arrive.time;
				slideData.type = ConvertMirrorSlide(m2sfileNotetypeID.getSlidetype());
				_note._noteData.Add(noteData);
				break;
			}
			case NoteTypeID.Def.TouchHold:
			{
				noteData.touchArea = rec.getTouchHoldArea();
				if (noteData.touchArea == NoteTypeID.TouchArea.C)
				{
					noteData.startButtonPos = 0;
				}
				noteData.effect = rec.getTouchHoldEffect();
				noteData.touchSize = rec.getTouchHoldSize();
				int holdLen = rec.getHoldLen();
				noteData.end.init(rec.getBar(), rec.getGrid() + holdLen, this);
				_note._noteData.Add(noteData);
				break;
			}
			default:
				result = false;
				break;
			}
			return result;
		}

		protected bool loadTotal(M2SRecord rec, int index)
		{
			int num = (int)(rec.getType().getValue() - 47);
			if (num >= 46)
			{
				return false;
			}
			_total._totalData[num] = (uint)rec.getTotalNum();
			return true;
		}

		protected void calcEach()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _note._noteData.Count; i++)
			{
				NoteData noteData = _note._noteData[i];
				if (i + 1 >= _note._noteData.Count || 1 == 0)
				{
					continue;
				}
				for (int j = i + 1; j < _note._noteData.Count; j++)
				{
					NoteData noteData2 = _note._noteData[j];
					if (noteData.time < noteData2.time)
					{
						break;
					}
					if (!(noteData2.time == noteData.time) || !IsEach(noteData2.type, noteData.type))
					{
						continue;
					}
					noteData.isEach = true;
					noteData2.isEach = true;
					if (IsEachLink(noteData2.type, noteData.type))
					{
						if (noteData.parent == null)
						{
							noteData2.parent = noteData;
							noteData.indexEach = num++;
							noteData2.indexEach = noteData.indexEach;
							noteData.eachChild.Add(noteData2);
						}
					}
					else
					{
						if (!IsEachTouchTapLink(noteData2.type, noteData.type) || noteData.parent != null)
						{
							continue;
						}
						List<NoteData> list = new List<NoteData>();
						for (int k = j; k < _note._noteData.Count; k++)
						{
							NoteData noteData3 = _note._noteData[k];
							if (noteData.time < noteData3.time)
							{
								break;
							}
							if (IsEachTouchTapLink(noteData3.type, noteData.type))
							{
								list.Add(noteData3);
							}
						}
						int checkIndex = 0;
						List<NoteData> list2 = new List<NoteData>();
						list2.Clear();
						list2.Add(noteData);
						CalcTouchTapEachSub(list, list2, ref checkIndex);
						if (list2.Count <= 1)
						{
							list2.Clear();
							continue;
						}
						noteData.eachChild = list2;
						noteData.indexTouchGroup = num2++;
						noteData.indexEach = num++;
						TouchChainList touchChainList = new TouchChainList
						{
							RootNoteId = noteData.indexTouchGroup
						};
						foreach (NoteData item in list2)
						{
							item.parent = noteData;
							item.indexTouchGroup = noteData.indexTouchGroup;
							item.indexEach = noteData.indexEach;
							touchChainList.Add(item.indexNote);
						}
						_note._touchChainList.Add(touchChainList);
					}
				}
			}
		}

		protected void CalcTouchTapEachSub(List<NoteData> tempList, List<NoteData> linkList, ref int checkIndex)
		{
			for (int i = 0; i < tempList.Count; i++)
			{
				if (IsTouchNext(tempList[i], linkList[checkIndex]) && !linkList.Contains(tempList[i]))
				{
					linkList.Add(tempList[i]);
				}
			}
			checkIndex++;
			if (linkList.Count > checkIndex)
			{
				CalcTouchTapEachSub(tempList, linkList, ref checkIndex);
			}
		}

		protected bool IsTouchNext(NoteData temp, NoteData link)
		{
			int linkButtonPos = link.startButtonPos;
			int tempButtonPos = temp.startButtonPos;
			bool result = false;
			int[] array = null;
			switch (link.touchArea)
			{
			case NoteTypeID.TouchArea.B:
				if (temp.touchArea == NoteTypeID.TouchArea.B)
				{
					array = new int[2] { 1, 7 };
				}
				else if (temp.touchArea == NoteTypeID.TouchArea.E)
				{
					array = new int[2] { 0, 1 };
				}
				else if (temp.touchArea == NoteTypeID.TouchArea.C)
				{
					array = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
				}
				break;
			case NoteTypeID.TouchArea.E:
				if (temp.touchArea == NoteTypeID.TouchArea.B)
				{
					array = new int[2] { 0, 7 };
				}
				break;
			case NoteTypeID.TouchArea.C:
				if (temp.touchArea == NoteTypeID.TouchArea.B)
				{
					array = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
				}
				break;
			}
			if (array != null && array.Any((int t) => tempButtonPos == (t + linkButtonPos) % 8))
			{
				result = true;
			}
			return result;
		}

		protected void calcSlide()
		{
			for (int i = 0; i < _note._noteData.Count; i++)
			{
				NoteData noteData = _note._noteData[i];
				if ((noteData.type == NoteTypeID.Def.Star || noteData.type == NoteTypeID.Def.BreakStar || noteData.type == NoteTypeID.Def.ExStar) && ((i + 1 < _note._noteData.Count) ? true : false))
				{
					for (int j = i + 1; j < _note._noteData.Count; j++)
					{
						NoteData noteData2 = _note._noteData[j];
						if (noteData2.type == NoteTypeID.Def.Slide && noteData2.time == noteData.time && noteData2.startButtonPos == noteData.startButtonPos)
						{
							noteData2.parent = noteData;
							noteData.child.Add(noteData2);
						}
					}
				}
				if (!(noteData.type == NoteTypeID.Def.Slide) || i + 1 >= _note._noteData.Count || 1 == 0)
				{
					continue;
				}
				for (int k = i; k < _note._noteData.Count; k++)
				{
					NoteData noteData3 = _note._noteData[k];
					if (noteData3.type == NoteTypeID.Def.Slide && noteData3.time == noteData.end && noteData3.startButtonPos == noteData.startButtonPos)
					{
						noteData3.parent = noteData.parent;
						noteData.child.Add(noteData3);
					}
				}
			}
		}

		protected int ConvertMirrorPosition(int position)
		{
			return _mirrorPos[(int)Singleton<GamePlayManager>.Instance.GetGameScore(_playerID).UserOption.MirrorMode, position];
		}

		protected int ConvertMirrorTouchEPosition(int position)
		{
			return _mirrorPosTouchE[(int)Singleton<GamePlayManager>.Instance.GetGameScore(_playerID).UserOption.MirrorMode, position];
		}

		protected SlideType ConvertMirrorSlide(SlideType slide)
		{
			return mirrorSlide[(int)Singleton<GamePlayManager>.Instance.GetGameScore(_playerID).UserOption.MirrorMode, (int)slide];
		}

		protected int getProgJudgeGrid(float bpm, float boost)
		{
			float num = 4f * _header._progJudgeBPM * boost;
			int num2 = 384;
			while (bpm < num)
			{
				bpm *= 2f;
				num2 /= 2;
				if (num2 <= 3)
				{
					break;
				}
			}
			return num2;
		}

		protected void calcNoteTiming()
		{
			foreach (MeterChangeData meter in _composition._meterList)
			{
				meter.time.calcMsec(this);
			}
			foreach (SoflanData soflan in _composition._soflanList)
			{
				soflan.time.calcMsec(this);
			}
			foreach (ClickData click in _composition._clickList)
			{
				click.time.calcMsec(this);
			}
			BarDataList[] barList = _composition._barList;
			for (int i = 0; i < barList.Length; i++)
			{
				foreach (BarData item in barList[i])
				{
					item.time.calcMsec(this);
				}
			}
			foreach (NoteData noteDatum in _note._noteData)
			{
				noteDatum.time.calcMsec(this);
				noteDatum.end.calcMsec(this);
				if (noteDatum.type == NoteTypeID.Def.Slide)
				{
					noteDatum.slideData.time.calcMsec(this);
				}
			}
		}

		protected void calcEndTiming()
		{
			NotesTime notesTime = new NotesTime(getResolution() * 16);
			NotesTime a = new NotesTime(getResolution() * 16);
			NotesTime notesTime2 = new NotesTime(0);
			NotesTime a2 = new NotesTime(0);
			foreach (NoteData noteDatum in _note._noteData)
			{
				switch (noteDatum.type.getValue())
				{
				case NoteTypeID.Def.Begin:
				case NoteTypeID.Def.Star:
				case NoteTypeID.Def.Break:
				case NoteTypeID.Def.BreakStar:
				case NoteTypeID.Def.TouchTap:
				case NoteTypeID.Def.ExTap:
				case NoteTypeID.Def.ExStar:
					notesTime = timeMin(notesTime, noteDatum.time);
					notesTime2 = timeMax(notesTime2, noteDatum.time);
					break;
				case NoteTypeID.Def.Hold:
				case NoteTypeID.Def.ExHold:
					notesTime = timeMin(notesTime, noteDatum.time);
					notesTime2 = timeMax(notesTime2, noteDatum.end);
					break;
				case NoteTypeID.Def.Slide:
					notesTime = timeMin(notesTime, noteDatum.time);
					notesTime2 = timeMax(notesTime2, noteDatum.end);
					break;
				case NoteTypeID.Def.TouchHold:
					notesTime = timeMin(notesTime, noteDatum.time);
					notesTime2 = timeMax(notesTime2, noteDatum.end);
					break;
				}
			}
			a = timeMin(a, notesTime);
			a2 = timeMax(a2, notesTime2);
			foreach (BPMChangeData bpm in _composition._bpmList)
			{
				a = timeMin(a, bpm.time);
				a2 = timeMax(a2, bpm.time);
			}
			foreach (MeterChangeData meter in _composition._meterList)
			{
				a = timeMin(a, meter.time);
				a2 = timeMax(a2, meter.time);
			}
			foreach (SoflanData soflan in _composition._soflanList)
			{
				a = timeMin(a, soflan.time);
				a2 = timeMax(a2, soflan.time);
			}
			foreach (ClickData click in _composition._clickList)
			{
				a = timeMin(a, click.time);
				a2 = timeMax(a2, click.time);
			}
			_composition._startGameTime = notesTime;
			_composition._startNotesTime = a;
			_composition._endGameTime = notesTime2;
			_composition._endNotesTime = a2;
		}

		protected void calcBPMList()
		{
			if (_composition._bpmList.Empty())
			{
				return;
			}
			_composition._bpmList.Sort((BPMChangeData x, BPMChangeData y) => x.time.CompareTo(y.time));
			NotesTime notesTime = default(NotesTime);
			float bpm = _composition._bpmList[0].bpm;
			notesTime.setMsec(Singleton<GamePlayManager>.Instance.GetGameScore(_playerID).UserOption.GetAdjustMSec());
			foreach (BPMChangeData bpm2 in _composition._bpmList)
			{
				NotesTime notesTime2 = bpm2.time - notesTime;
				float msec = notesTime.msec + notesTime2.getFourBeat(getResolution()) * 60000f / bpm;
				bpm2.time.setMsec(msec);
				notesTime = bpm2.time;
				bpm = bpm2.bpm;
			}
			List<NotesTime> list = new List<NotesTime>();
			foreach (MeterChangeData meter in _composition._meterList)
			{
				list.Add(meter.time);
			}
			foreach (SoflanData soflan in _composition._soflanList)
			{
				list.Add(soflan.time);
			}
			foreach (ClickData click in _composition._clickList)
			{
				list.Add(click.time);
			}
			foreach (NoteData noteDatum in _note._noteData)
			{
				switch (noteDatum.type.getEnum())
				{
				case NoteTypeID.Def.Begin:
				case NoteTypeID.Def.Star:
				case NoteTypeID.Def.Break:
				case NoteTypeID.Def.BreakStar:
				case NoteTypeID.Def.TouchTap:
				case NoteTypeID.Def.ExTap:
				case NoteTypeID.Def.ExStar:
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.Hold:
				case NoteTypeID.Def.ExHold:
					list.Add(noteDatum.time);
					list.Add(noteDatum.end);
					break;
				case NoteTypeID.Def.Slide:
					list.Add(noteDatum.time);
					list.Add(noteDatum.end);
					list.Add(noteDatum.slideData.shoot.time);
					list.Add(noteDatum.slideData.arrive.time);
					break;
				case NoteTypeID.Def.TouchHold:
					list.Add(noteDatum.time);
					list.Add(noteDatum.end);
					break;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				NotesTime value = list[i];
				value.calcMsec(this);
				list[i] = value;
			}
		}

		protected void calcBPMInfo()
		{
			BPMChangeDataList bpmList = _composition._bpmList;
			if (bpmList.Empty())
			{
				return;
			}
			bpmList.Sort((BPMChangeData x, BPMChangeData y) => x.time.CompareTo(y.time));
			BPMInfo bpmInfo = _header._bpmInfo;
			bpmInfo.minBPM = (bpmInfo.maxBPM = (bpmInfo.defaultBPM = (bpmInfo.firstBPM = bpmList[0].bpm)));
			if (bpmList.Count() == 1)
			{
				return;
			}
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < bpmList.Count(); i++)
			{
				BPMChangeData bPMChangeData = bpmList[i];
				float num = 0f;
				num = ((i != bpmList.Count() - 1) ? (bpmList[i + 1].time.msec - bPMChangeData.time.msec) : (_composition._endNotesTime.msec - bPMChangeData.time.msec));
				if (bpmInfo.maxBPM < bPMChangeData.bpm)
				{
					bpmInfo.maxBPM = bPMChangeData.bpm;
				}
				if (bpmInfo.minBPM > bPMChangeData.bpm)
				{
					bpmInfo.minBPM = bPMChangeData.bpm;
				}
				int key = (int)Math.Floor(bPMChangeData.bpm * 1000f + 0.5f);
				if (dictionary.ContainsKey(key))
				{
					dictionary[key] += num;
				}
				else
				{
					dictionary.Add(key, num);
				}
			}
			int num2 = 0;
			float num3 = 0f;
			int num4 = 0;
			foreach (KeyValuePair<int, float> item in dictionary)
			{
				if (num4++ == 0 || num3 < item.Value)
				{
					num2 = item.Key;
					num3 = item.Value;
				}
			}
			bpmInfo.defaultBPM = 0.001f * (float)num2;
		}

		protected void calcSoflanList()
		{
			_composition._soflanList.Sort((SoflanData x, SoflanData y) => x.time.CompareTo(y.time));
		}

		protected void calcClickList()
		{
			_composition._clickSeList.Clear();
			NotesTime notesTime = default(NotesTime);
			notesTime.init(0, _header._clickFirst, this);
			foreach (BarData item in _composition._barList[1])
			{
				if (item.time < notesTime)
				{
					ClickData clickData = new ClickData();
					clickData.time = item.time;
					_composition._clickSeList.Add(clickData);
					continue;
				}
				break;
			}
			foreach (ClickData click in _composition._clickList)
			{
				_composition._clickSeList.Add(click);
			}
			_composition._clickSeList.Sort((ClickData x, ClickData y) => x.time.CompareTo(y.time));
		}

		protected void calcTotal()
		{
			_total.clear();
			List<NotesTime> list = new List<NotesTime>();
			NoteDataList noteDataList = new NoteDataList();
			uint judgeScore = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical);
			uint judgeScore2 = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.Hold);
			uint judgeScore3 = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.Slide);
			uint judgeScore4 = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.Break);
			uint judgeScore5 = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.BreakBonus);
			foreach (NoteData noteDatum in _note._noteData)
			{
				switch (noteDatum.type.getEnum())
				{
				case NoteTypeID.Def.ExTap:
					_total._totalData[8]++;
					_total._totalData[20]++;
					_total._totalData[32]++;
					_total._totalData[36]++;
					_total._totalData[43] += judgeScore;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.Begin:
					_total._totalData[0]++;
					_total._totalData[12]++;
					_total._totalData[24]++;
					_total._totalData[36]++;
					_total._totalData[43] += judgeScore;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.Break:
					_total._totalData[1]++;
					_total._totalData[13]++;
					_total._totalData[25]++;
					_total._totalData[37]++;
					_total._totalData[43] += judgeScore4;
					_total._totalData[44] += judgeScore5;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.Hold:
				{
					_total._totalData[2]++;
					_total._totalData[14]++;
					_total._totalData[26]++;
					_total._totalData[38]++;
					_total._totalData[43] += judgeScore2;
					list.Add(noteDatum.time);
					NoteData noteData3 = noteDatum;
					noteData3.option = 1;
					noteDataList.Add(noteData3);
					list.Add(noteData3.time);
					break;
				}
				case NoteTypeID.Def.ExHold:
				{
					_total._totalData[9]++;
					_total._totalData[21]++;
					_total._totalData[33]++;
					_total._totalData[38]++;
					_total._totalData[43] += judgeScore2;
					list.Add(noteDatum.time);
					NoteData noteData2 = noteDatum;
					noteData2.option = 1;
					noteDataList.Add(noteData2);
					list.Add(noteData2.time);
					break;
				}
				case NoteTypeID.Def.Slide:
					_total._totalData[5]++;
					_total._totalData[17]++;
					_total._totalData[29]++;
					_total._totalData[39]++;
					_total._totalData[43] += judgeScore3;
					break;
				case NoteTypeID.Def.Star:
					_total._totalData[3]++;
					_total._totalData[15]++;
					_total._totalData[27]++;
					_total._totalData[36]++;
					_total._totalData[43] += judgeScore;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.ExStar:
					_total._totalData[10]++;
					_total._totalData[22]++;
					_total._totalData[34]++;
					_total._totalData[36]++;
					_total._totalData[43] += judgeScore;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.BreakStar:
					_total._totalData[4]++;
					_total._totalData[16]++;
					_total._totalData[28]++;
					_total._totalData[37]++;
					_total._totalData[43] += judgeScore4;
					_total._totalData[44] += judgeScore5;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.TouchTap:
					_total._totalData[6]++;
					_total._totalData[18]++;
					_total._totalData[30]++;
					_total._totalData[40]++;
					_total._totalData[43] += judgeScore;
					list.Add(noteDatum.time);
					break;
				case NoteTypeID.Def.TouchHold:
				{
					_total._totalData[7]++;
					_total._totalData[19]++;
					_total._totalData[31]++;
					_total._totalData[38]++;
					_total._totalData[43] += judgeScore2;
					list.Add(noteDatum.time);
					NoteData noteData = noteDatum;
					noteData.option = 1;
					noteDataList.Add(noteData);
					list.Add(noteData.time);
					break;
				}
				}
			}
			for (int i = 0; i < 11; i++)
			{
				_total._totalData[11] += _total._totalData[i];
			}
			for (int j = 12; j < 23; j++)
			{
				_total._totalData[23] += _total._totalData[j];
			}
			for (int k = 36; k < 41; k++)
			{
				_total._totalData[41] += _total._totalData[k];
			}
			_total._totalData[45] = _total._totalData[43] + _total._totalData[44];
		}

		public void calcBarList()
		{
			int num = getResolution();
			if (num == 0)
			{
				num = 1920;
			}
			BarDataList[] barList = _composition._barList;
			for (int i = 0; i < barList.Length; i++)
			{
				barList[i].Clear();
			}
			_composition._meterList.Sort((MeterChangeData x, MeterChangeData y) => x.time.CompareTo(y.time));
			if (_composition._meterList.Count >= 2)
			{
				int num2 = 0;
				while (num2 < _composition._meterList.Count - 1)
				{
					if (_composition._meterList[num2].time == _composition._meterList[num2 + 1].time)
					{
						_composition._meterList.RemoveAt(num2);
					}
					else
					{
						num2++;
					}
				}
			}
			bool flag = false;
			if (!_composition._meterList.Empty() && _composition._meterList.First().time.grid == 0)
			{
				flag = true;
			}
			if (!flag)
			{
				MeterChangeData meterChangeData = new MeterChangeData();
				meterChangeData.time.init(0, 0, this);
				meterChangeData.num = _header._metInfo.num;
				meterChangeData.denomi = _header._metInfo.denomi;
				_composition._meterList.Insert(0, meterChangeData);
			}
			NotesTime endNotesTime = _composition._endNotesTime;
			uint num3 = (uint)(endNotesTime.grid / num + 2);
			for (uint num4 = 0u; num4 < num3; num4++)
			{
				BarData barData = new BarData();
				barData.numBar = 0u;
				barData.numTotal = num4;
				barData.time.init((int)num4, 0, this);
				_composition._barList[0].Add(barData);
			}
			uint num5 = 0u;
			for (int j = 0; j < _composition._barList[0].Count; j++)
			{
				BarData barData2 = _composition._barList[0][j];
				int grid = endNotesTime.grid;
				if (j + 1 < _composition._barList[0].Count)
				{
					grid = _composition._barList[0][j + 1].time.grid;
				}
				uint num6 = 0u;
				BarData barData3 = new BarData();
				barData3.numBar = num6;
				barData3.numTotal = num5;
				barData3.time = barData2.time;
				_composition._barList[1].Add(barData3);
				num6++;
				num5++;
				int num7 = num / 4;
				for (int k = barData2.time.grid + num7; k < grid; k += num7)
				{
					BarData barData4 = new BarData();
					barData4.numBar = num6;
					barData4.numTotal = num5;
					barData4.time.init(0, k, this);
					_composition._barList[1].Add(barData4);
					num6++;
					num5++;
				}
			}
			uint num8 = 0u;
			List<BarData2> list = new List<BarData2>();
			for (int l = 0; l < _composition._meterList.Count; l++)
			{
				MeterChangeData meterChangeData2 = _composition._meterList[l];
				int num9 = num * (endNotesTime.grid / num + 1) + 1;
				if (l + 1 < _composition._meterList.Count)
				{
					num9 = _composition._meterList[l + 1].time.grid;
				}
				BarData2 barData5 = new BarData2();
				barData5.meterData = meterChangeData2;
				BarData barData6 = barData5.barData;
				barData6.numBar = 0u;
				barData6.numTotal = num8;
				barData6.time = meterChangeData2.time;
				list.Add(barData5);
				num8++;
				if (meterChangeData2.num == 0 || meterChangeData2.denomi == 0)
				{
					break;
				}
				int num10 = (int)(num * meterChangeData2.num / (long)meterChangeData2.denomi);
				for (int m = meterChangeData2.time.grid + num10; m < num9; m += num10)
				{
					BarData2 barData7 = new BarData2();
					barData7.meterData = meterChangeData2;
					barData7.barData.numBar = 0u;
					barData7.barData.numTotal = num8;
					barData7.barData.time.init(0, m, this);
					list.Add(barData7);
					num8++;
				}
			}
			foreach (BarData2 item in list)
			{
				_composition._barList[2].Add(item.barData);
			}
			uint num11 = 0u;
			for (int n = 0; n < list.Count; n++)
			{
				BarData2 barData8 = list[n];
				int grid2 = endNotesTime.grid;
				if (n + 1 < list.Count)
				{
					grid2 = list[n + 1].barData.time.grid;
				}
				uint num12 = 0u;
				BarData barData9 = new BarData();
				barData9.numBar = num12;
				barData9.numTotal = num11;
				barData9.time = barData8.barData.time;
				_composition._barList[3].Add(barData9);
				num11++;
				num12++;
				if (barData8.meterData.num != 0 && barData8.meterData.denomi != 0)
				{
					int num13 = (int)((long)num / (long)barData8.meterData.denomi);
					for (int num14 = barData8.barData.time.grid + num13; num14 < grid2; num14 += num13)
					{
						BarData barData10 = new BarData();
						barData10.numBar = num12;
						barData10.numTotal = num11;
						barData10.time.init(0, num14, this);
						_composition._barList[3].Add(barData10);
						num11++;
						num12++;
					}
					continue;
				}
				break;
			}
		}

		protected FormatType checkFormat(string fileName)
		{
			return FormatType.FORMAT_M2S;
		}

		public float calcMsec(NotesTime timing)
		{
			foreach (BPMChangeData item in Enumerable.Reverse(_composition._bpmList))
			{
				if (!(item.time > timing))
				{
					return item.time.msec + (timing - item.time).getFourBeat(getResolution()) * 60000f / item.bpm;
				}
			}
			float num = Mathf.Max(_header._bpmInfo.firstBPM, 0.01f);
			return timing.getFourBeat(getResolution()) * 60000f / num + Singleton<GamePlayManager>.Instance.GetGameScore(_playerID).UserOption.GetAdjustMSec();
		}

		public float calcFrame(NotesTime timing)
		{
			return calcMsec(timing) * 0.06f;
		}

		public float calcGridByMsec(float msec)
		{
			if (_composition._bpmList.Empty())
			{
				return 0f;
			}
			BPMChangeData bPMChangeData = _composition._bpmList.First();
			foreach (BPMChangeData item in Enumerable.Reverse(_composition._bpmList))
			{
				if (!(item.time.msec > msec))
				{
					bPMChangeData = item;
					break;
				}
			}
			double num = (double)(msec - bPMChangeData.time.msec) * (double)getResolution() * (double)bPMChangeData.bpm / 240000.0;
			return (float)bPMChangeData.time.grid + (float)num;
		}

		public bool IsEach(NoteTypeID type1, NoteTypeID type2)
		{
			if (type1 == NoteTypeID.Def.Slide)
			{
				if (type2 == NoteTypeID.Def.Slide)
				{
					return true;
				}
			}
			else if (type2 != NoteTypeID.Def.Slide)
			{
				return true;
			}
			return false;
		}

		public bool IsSlide(NoteTypeID type1)
		{
			NoteTypeID.Def value = type1.getValue();
			if (value == NoteTypeID.Def.Slide)
			{
				return true;
			}
			return false;
		}

		public bool IsTouch(NoteTypeID type1)
		{
			NoteTypeID.Def value = type1.getValue();
			if ((uint)(value - 6) <= 1u)
			{
				return true;
			}
			return false;
		}

		public bool IsTouchTap(NoteTypeID type1)
		{
			NoteTypeID.Def value = type1.getValue();
			if (value == NoteTypeID.Def.TouchTap)
			{
				return true;
			}
			return false;
		}

		public bool IsNormalTap(NoteTypeID type1)
		{
			if (!IsSlide(type1))
			{
				return !IsTouch(type1);
			}
			return false;
		}

		public bool IsEachLink(NoteTypeID type1, NoteTypeID type2)
		{
			if (!IsNormalTap(type1))
			{
				return false;
			}
			if (!IsNormalTap(type2))
			{
				return false;
			}
			return true;
		}

		public bool IsEachTouchTapLink(NoteTypeID type1, NoteTypeID type2)
		{
			if (!IsTouchTap(type1))
			{
				return false;
			}
			if (!IsTouchTap(type2))
			{
				return false;
			}
			return true;
		}

		protected void set(NotesReader r)
		{
			_loadType = r._loadType;
			_header = r._header;
			_composition = r._composition;
			_note = r._note;
			_total = r._total;
		}

		private NotesTime timeMin(NotesTime a, NotesTime b)
		{
			if (!(a < b))
			{
				return b;
			}
			return a;
		}

		private NotesTime timeMax(NotesTime a, NotesTime b)
		{
			if (!(a > b))
			{
				return b;
			}
			return a;
		}
	}
}
