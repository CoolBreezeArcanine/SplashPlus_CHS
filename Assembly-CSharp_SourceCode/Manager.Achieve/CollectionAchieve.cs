using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.UserDatas;
using Net.VO.Mai2;

namespace Manager.Achieve
{
	public class CollectionAchieve : Singleton<CollectionAchieve>
	{
		private readonly Func<int, object, ReleaseConditions, bool>[] _checks;

		private ILookup<int, KeyValuePair<int, PlateData>> _srcPlates;

		private ILookup<int, KeyValuePair<int, TitleData>> _srcTitles;

		private ILookup<int, KeyValuePair<int, IconData>> _srcIcons;

		private ILookup<int, KeyValuePair<int, PartnerData>> _srcPartners;

		private ILookup<int, KeyValuePair<int, FrameData>> _srcFrames;

		private Tuple<int[], Dictionary<int, ReleaseConditions>>[] _condsPlate;

		private Tuple<int[], Dictionary<int, ReleaseConditions>>[] _condsTitle;

		private Tuple<int[], Dictionary<int, ReleaseConditions>>[] _condsIcon;

		private Tuple<int[], Dictionary<int, ReleaseConditions>>[] _condsPartner;

		private Tuple<int[], Dictionary<int, ReleaseConditions>>[] _condsFrame;

		private UserData _userData;

		private AchieveTrackData _track;

		private AchieveCreditData _credit;

		private AchieveTotalData _total;

		public CollectionAchieve()
		{
			_checks = new Func<int, object, ReleaseConditions, bool>[3]
			{
				AchieveTrack.Checks,
				AchieveCredit.Checks,
				AchieveTotal.Checks
			};
		}

		public void Configure()
		{
			_srcPlates = (from i in Singleton<DataManager>.Instance.GetPlates()
				where i.Value.relConds.GetItemConditions().Any((ReleaseItemConditions j) => j.kindTrack != 0 || j.kindCredit != 0 || j.kindTotal != ReleaseConditionTotalKind.None)
				select i).ToLookup((KeyValuePair<int, PlateData> i) => i.Value.eventName.id);
			_srcTitles = (from i in Singleton<DataManager>.Instance.GetTitles()
				where i.Value.relConds.GetItemConditions().Any((ReleaseItemConditions j) => j.kindTrack != 0 || j.kindCredit != 0 || j.kindTotal != ReleaseConditionTotalKind.None)
				select i).ToLookup((KeyValuePair<int, TitleData> i) => i.Value.eventName.id);
			_srcIcons = (from i in Singleton<DataManager>.Instance.GetIcons()
				where i.Value.relConds.GetItemConditions().Any((ReleaseItemConditions j) => j.kindTrack != 0 || j.kindCredit != 0 || j.kindTotal != ReleaseConditionTotalKind.None)
				select i).ToLookup((KeyValuePair<int, IconData> i) => i.Value.eventName.id);
			_srcPartners = (from i in Singleton<DataManager>.Instance.GetPartners()
				where i.Value.relConds.GetItemConditions().Any((ReleaseItemConditions j) => j.kindTrack != 0 || j.kindCredit != 0 || j.kindTotal != ReleaseConditionTotalKind.None)
				select i).ToLookup((KeyValuePair<int, PartnerData> i) => i.Value.eventName.id);
			_srcFrames = (from i in Singleton<DataManager>.Instance.GetFrames()
				where i.Value.relConds.GetItemConditions().Any((ReleaseItemConditions j) => j.kindTrack != 0 || j.kindCredit != 0 || j.kindTotal != ReleaseConditionTotalKind.None)
				select i).ToLookup((KeyValuePair<int, FrameData> i) => i.Value.eventName.id);
		}

		public void Initialize()
		{
			if (!GameManager.IsEventMode)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				Dictionary<int, object> src = _srcPlates.Where((IGrouping<int, KeyValuePair<int, PlateData>> i) => Singleton<EventManager>.Instance.IsOpenEvent(i.Key)).SelectMany((IGrouping<int, KeyValuePair<int, PlateData>> i) => i).ToDictionary((Func<KeyValuePair<int, PlateData>, int>)((KeyValuePair<int, PlateData> i) => i.Key), (Func<KeyValuePair<int, PlateData>, object>)((KeyValuePair<int, PlateData> i) => i.Value));
				Dictionary<int, object> src2 = _srcTitles.Where((IGrouping<int, KeyValuePair<int, TitleData>> i) => Singleton<EventManager>.Instance.IsOpenEvent(i.Key)).SelectMany((IGrouping<int, KeyValuePair<int, TitleData>> i) => i).ToDictionary((Func<KeyValuePair<int, TitleData>, int>)((KeyValuePair<int, TitleData> i) => i.Key), (Func<KeyValuePair<int, TitleData>, object>)((KeyValuePair<int, TitleData> i) => i.Value));
				Dictionary<int, object> src3 = _srcIcons.Where((IGrouping<int, KeyValuePair<int, IconData>> i) => Singleton<EventManager>.Instance.IsOpenEvent(i.Key)).SelectMany((IGrouping<int, KeyValuePair<int, IconData>> i) => i).ToDictionary((Func<KeyValuePair<int, IconData>, int>)((KeyValuePair<int, IconData> i) => i.Key), (Func<KeyValuePair<int, IconData>, object>)((KeyValuePair<int, IconData> i) => i.Value));
				Dictionary<int, object> src4 = _srcPartners.Where((IGrouping<int, KeyValuePair<int, PartnerData>> i) => Singleton<EventManager>.Instance.IsOpenEvent(i.Key)).SelectMany((IGrouping<int, KeyValuePair<int, PartnerData>> i) => i).ToDictionary((Func<KeyValuePair<int, PartnerData>, int>)((KeyValuePair<int, PartnerData> i) => i.Key), (Func<KeyValuePair<int, PartnerData>, object>)((KeyValuePair<int, PartnerData> i) => i.Value));
				Dictionary<int, object> src5 = _srcFrames.Where((IGrouping<int, KeyValuePair<int, FrameData>> i) => Singleton<EventManager>.Instance.IsOpenEvent(i.Key)).SelectMany((IGrouping<int, KeyValuePair<int, FrameData>> i) => i).ToDictionary((Func<KeyValuePair<int, FrameData>, int>)((KeyValuePair<int, FrameData> i) => i.Key), (Func<KeyValuePair<int, FrameData>, object>)((KeyValuePair<int, FrameData> i) => i.Value));
				_condsPlate = ToTable(ToConditions(src, (object i) => ((PlateData)((KeyValuePair<int, object>)i).Value).relConds));
				_condsTitle = ToTable(ToConditions(src2, (object i) => ((TitleData)((KeyValuePair<int, object>)i).Value).relConds));
				_condsIcon = ToTable(ToConditions(src3, (object i) => ((IconData)((KeyValuePair<int, object>)i).Value).relConds));
				_condsPartner = ToTable(ToConditions(src4, (object i) => ((PartnerData)((KeyValuePair<int, object>)i).Value).relConds));
				_condsFrame = ToTable(ToConditions(src5, (object i) => ((FrameData)((KeyValuePair<int, object>)i).Value).relConds));
				stopwatch.Stop();
			}
		}

		private static Tuple<int[], Dictionary<int, ReleaseConditions>>[] ToTable(IEnumerable<Dictionary<int, ReleaseConditions>> src)
		{
			return src.Select((Dictionary<int, ReleaseConditions> i) => new Tuple<int[], Dictionary<int, ReleaseConditions>>(i.Keys.ToArray(), i)).ToArray();
		}

		private static IEnumerable<Dictionary<int, ReleaseConditions>> ToConditions(Dictionary<int, object> src, Func<object, ReleaseConditions> func)
		{
			IEnumerable<IGrouping<ReleaseConditionCategory, KeyValuePair<int, object>>> enumerable = from i in src
				group i by func(i).category;
			Dictionary<int, ReleaseConditions>[] array = new Dictionary<int, ReleaseConditions>[3]
			{
				new Dictionary<int, ReleaseConditions>(),
				new Dictionary<int, ReleaseConditions>(),
				new Dictionary<int, ReleaseConditions>()
			};
			foreach (IGrouping<ReleaseConditionCategory, KeyValuePair<int, object>> item in enumerable)
			{
				array[(int)item.Key] = item.ToDictionary((KeyValuePair<int, object> i) => i.Key, (KeyValuePair<int, object> i) => func(i));
			}
			return array;
		}

		public void BuildParameter(int index, ReleaseConditionCategory category)
		{
			_userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			switch (category)
			{
			case ReleaseConditionCategory.Track:
				_track = AchieveTrackData.Create(index);
				break;
			case ReleaseConditionCategory.Credit:
				_credit = AchieveCreditData.Create(index);
				break;
			case ReleaseConditionCategory.Total:
				_total = AchieveTotalData.Create(index);
				break;
			}
		}

		public int[] CheckNewPlate(ReleaseConditionCategory category)
		{
			return CheckConditions(ItemKind.Plate, category, _userData.PlateList.Select((Manager.UserDatas.UserItem i) => i.itemId).ToList());
		}

		public int[] CheckNewTitle(ReleaseConditionCategory category)
		{
			return CheckConditions(ItemKind.Title, category, _userData.TitleList.Select((Manager.UserDatas.UserItem i) => i.itemId).ToList());
		}

		public int[] CheckNewIcon(ReleaseConditionCategory category)
		{
			return CheckConditions(ItemKind.Icon, category, _userData.IconList.Select((Manager.UserDatas.UserItem i) => i.itemId).ToList());
		}

		public int[] CheckNewPartner(ReleaseConditionCategory category)
		{
			return CheckConditions(ItemKind.Partner, category, _userData.PartnerList.Select((Manager.UserDatas.UserItem i) => i.itemId).ToList());
		}

		public int[] CheckNewFrame(ReleaseConditionCategory category)
		{
			return CheckConditions(ItemKind.Frame, category, _userData.FrameList.Select((Manager.UserDatas.UserItem i) => i.itemId).ToList());
		}

		private int[] CheckConditions(ItemKind kind, ReleaseConditionCategory category, IEnumerable<int> items)
		{
			Tuple<int[], Dictionary<int, ReleaseConditions>> table;
			switch (kind)
			{
			case ItemKind.Plate:
				table = _condsPlate[(int)category];
				break;
			case ItemKind.Title:
				table = _condsTitle[(int)category];
				break;
			case ItemKind.Icon:
				table = _condsIcon[(int)category];
				break;
			case ItemKind.Partner:
				table = _condsPartner[(int)category];
				break;
			case ItemKind.Frame:
				table = _condsFrame[(int)category];
				break;
			default:
				return Array.Empty<int>();
			}
			return (from i in table.Item1.Except(items)
				where Check(i, category, table.Item2[i])
				select i).ToArray();
		}

		private bool Check(int id, ReleaseConditionCategory category, ReleaseConditions conds)
		{
			return category switch
			{
				ReleaseConditionCategory.Track => _checks[(int)category](id, _track, conds), 
				ReleaseConditionCategory.Credit => _checks[(int)category](id, _credit, conds), 
				ReleaseConditionCategory.Total => _checks[(int)category](id, _total, conds), 
				_ => false, 
			};
		}
	}
}
