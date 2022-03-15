using System;
using System.Reflection;
using System.Text;

namespace MAI2.Util
{
	public class Mode<TClass, TEnum> where TEnum : struct, IConvertible
	{
		private struct Method
		{
			public Action<TClass> init_;

			public Action<TClass> proc_;

			public Action<TClass> term_;
		}

		private int prev_;

		private int current_;

		private TClass parent_;

		private Method[] methods_;

		private static Action<TClass> getMethod(Type type, string name)
		{
			MethodInfo method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null == method)
			{
				return null;
			}
			return Delegate.CreateDelegate(typeof(Action<TClass>), method) as Action<TClass>;
		}

		public Mode(TClass parent)
		{
			parent_ = parent;
			string[] names = Enum.GetNames(typeof(TEnum));
			methods_ = new Method[names.Length];
			Type typeFromHandle = typeof(TClass);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < names.Length; i++)
			{
				int length = names[i].Length;
				stringBuilder.Length = 0;
				methods_[i].init_ = getMethod(typeFromHandle, stringBuilder.Append(names[i]).Append("_Init").ToString());
				methods_[i].proc_ = getMethod(typeFromHandle, stringBuilder.Remove(length, 5).Append("_Proc").ToString());
				methods_[i].term_ = getMethod(typeFromHandle, stringBuilder.Remove(length, 5).Append("_Term").ToString());
			}
			init(0);
		}

		public bool valid()
		{
			return parent_ != null;
		}

		public int get()
		{
			return current_;
		}

		private void init(int state)
		{
			prev_ = -1;
			current_ = state;
		}

		public void init(TEnum state)
		{
			prev_ = -1;
			current_ = Convert.ToInt32(state);
		}

		public void set(TEnum state)
		{
			current_ = Convert.ToInt32(state);
		}

		public void term()
		{
			if (prev_ != current_)
			{
				initImpl();
			}
			while (0 <= prev_ && methods_[prev_].term_ != null)
			{
				int num = prev_;
				prev_ = current_;
				methods_[num].term_(parent_);
				if (prev_ == current_)
				{
					break;
				}
			}
			parent_ = default(TClass);
			methods_ = null;
		}

		public void update()
		{
			if (prev_ != current_)
			{
				initImpl();
				if (parent_ != null && methods_[current_].proc_ != null)
				{
					methods_[current_].proc_(parent_);
				}
			}
			else if (methods_[current_].proc_ != null)
			{
				methods_[current_].proc_(parent_);
			}
		}

		private void initImpl()
		{
			do
			{
				if (0 <= prev_ && methods_[prev_].term_ != null)
				{
					methods_[prev_].term_(parent_);
				}
				prev_ = current_;
				if (methods_[current_].init_ != null)
				{
					methods_[current_].init_(parent_);
				}
			}
			while (prev_ != current_);
		}
	}
}
