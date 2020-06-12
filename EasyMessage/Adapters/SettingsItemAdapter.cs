using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EasyMessage.Entities;

namespace EasyMessage.Adapters
{
    public class SettingsItemAdapter : BaseAdapter<SettingsItem>
	{
		protected IList<SettingsItem> _items;

		public SettingsItemAdapter(IList<SettingsItem> items)
		{
			_items = items;
		}
		public override SettingsItem this[int position]
		{
			get
			{
				return _items[position];
			}
		}


		public override int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item, parent, false);

			var val = view.FindViewById<TextView>(Resource.Id.i_value);
			var desc = view.FindViewById<TextView>(Resource.Id.i_desc);

			val.Text = _items[position].sett_value;
			desc.Text = _items[position].sett_description;

			return view;
		}

		class ViewHolder : Java.Lang.Object
		{
			public TextView Value { get; set; }
			public TextView Description { get; set; }
		}

		class SectionViewHolder : Java.Lang.Object
		{
			public TextView Text { get; set; }
		}
	}
}
