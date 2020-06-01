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
using Message = EasyMessage.Entities.Message;

namespace EasyMessage.Adapters
{
    public class DialogItemAdapter : BaseAdapter<Message>
	{
		protected IList<Message> _items;

		public DialogItemAdapter(IList<Message> items)
		{
			_items = items;
		}
		public override Message this[int position]
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
			var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.dialog_item, parent, false);

			var contact  = view.FindViewById<TextView>(Resource.Id.collocutor);
			var lastMess = view.FindViewById<TextView>(Resource.Id.mess);
			//var pic = view.FindViewById<ImageView>(Resource.Id.contactImage);

			contact.Text = _items[position].senderP;
			lastMess.Text = _items[position].contentP;
			//pic.SetBackgroundResource(Convert.ToInt32(_items[position].picturePathP));

			return view;
		}

		class ViewHolder : Java.Lang.Object
		{
			public TextView Collocutor { get; set; }
			public TextView LastMessage { get; set; }
			//public ImageView Picture { get; set; }
		}

		class SectionViewHolder : Java.Lang.Object
		{
			public TextView Text { get; set; }
		}
	}
}