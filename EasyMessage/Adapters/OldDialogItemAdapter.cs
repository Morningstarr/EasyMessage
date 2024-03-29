﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EasyMessage.Controllers;
using EasyMessage.Entities;

namespace EasyMessage.Adapters
{
    public class OldDialogItemAdapter : BaseAdapter<MyDialog>
	{
		protected IList<MyDialog> _items;
		protected bool isNew;

		public OldDialogItemAdapter(IList<MyDialog> items)
		{
			_items = items;
		}
		public OldDialogItemAdapter(IList<MyDialog> items, bool isn)
		{
			_items = items;
			isNew = isn;
		}
		public override MyDialog this[int position]
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

			if (_items[position].accessFlag != 0 && _items[position].accessFlag == 2 && _items[position].senderP != AccountsController.mainAccP.emailP)
			{
				view.SetBackgroundColor(Color.ParseColor("#0cd8f4")); 
				//view.
			}

			var contact = view.FindViewById<TextView>(Resource.Id.collocutor);
			var lastMess = view.FindViewById<TextView>(Resource.Id.mess);
			//var pic = view.FindViewById<ImageView>(Resource.Id.contactImage);

			contact.Text = _items[position].senderP == AccountsController.mainAccP.emailP ? _items[position].receiverP : _items[position].senderP;
			if (_items[position].messageFlag == 6 || _items[position].messageFlag == 5)
			{
				lastMess.Text ="*содержимое скрыто*";
			}
			else
			{
				lastMess.Text = _items[position].contentP;
			}
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
 