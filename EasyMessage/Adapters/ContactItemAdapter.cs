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
    public class ContactItemAdapter : BaseAdapter<Contact>
    {
		protected IList<Contact> _items;

		public ContactItemAdapter(IList<Contact> items)
		{
			_items = items;
		}
		public override Contact this[int position]
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
			var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.contact_item, parent, false);

			var mail = view.FindViewById<TextView>(Resource.Id.contactItemMail);
			var name = view.FindViewById<TextView>(Resource.Id.contactItemName);
			var pic = view.FindViewById<ImageView>(Resource.Id.contactImage);

			mail.Text = _items[position].contactAddressP;
			name.Text = _items[position].contactNameP;
			pic.SetBackgroundResource(Convert.ToInt32(_items[position].picturePathP));

			return view;
		}

		class ViewHolder : Java.Lang.Object
		{
			public TextView Email { get; set; }
			public TextView Name { get; set; }
			public ImageView Picture { get; set; }
		}

		class SectionViewHolder : Java.Lang.Object
		{
			public TextView Text { get; set; }
		}
	}
}