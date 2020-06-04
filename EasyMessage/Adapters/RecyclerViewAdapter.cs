using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Message = EasyMessage.Entities.Message;

namespace EasyMessage.Adapters
{
    public class RecyclerViewAdapter : RecyclerView.Adapter
    {
        private const int VIEW_TYPE_MESSAGE_SENT = 1;
        private const int VIEW_TYPE_MESSAGE_RECEIVED = 2;

        private IList<Message> _messages;
        public override int ItemCount
        {
            get { return _messages.Count; }
        }

        public RecyclerViewAdapter(IList<Message> messages)
        {
            _messages = messages;
        }

        /*public override Message this[int position]
        {
            get
            {
                return _messages[position];
            }
        }*/
        public override int GetItemViewType(int position)
        {
            Message message = _messages[position];

            if (message.senderP == AccountsController.mainAccP.emailP)
            {
                // If the current user is the sender of the message
                return VIEW_TYPE_MESSAGE_SENT;
            }
            else
            {
                // If some other user sent the message
                return VIEW_TYPE_MESSAGE_RECEIVED;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Message message = _messages[position];

            switch (GetItemViewType(position))
            {
                case VIEW_TYPE_MESSAGE_SENT:
                    ((SentMessageHolder)holder).Time.Text = Convert.ToDateTime(message.timeP).ToUniversalTime().TimeOfDay.ToString();
                    ((SentMessageHolder)holder).Content.Text = message.contentP;
                    break;
                case VIEW_TYPE_MESSAGE_RECEIVED:
                    ((ReceivedMessageHolder)holder).Time.Text = Convert.ToDateTime(message.timeP).ToUniversalTime().TimeOfDay.ToString();
                    ((ReceivedMessageHolder)holder).Content.Text = message.contentP;
                    ((ReceivedMessageHolder)holder).Sender.Text = message.senderP;
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view;

            if (viewType == VIEW_TYPE_MESSAGE_SENT)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.sent_message, parent, false);
                return new SentMessageHolder(view);
            }
            else if (viewType == VIEW_TYPE_MESSAGE_RECEIVED)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.received_message, parent, false);
                return new ReceivedMessageHolder(view);
            }

            return null;
        }

        private class ReceivedMessageHolder : RecyclerView.ViewHolder
        {
            public TextView Content { get; set; }
            public TextView Time { get; set; }
            public TextView Sender { get; set; }

            public ReceivedMessageHolder(View v) : base (v)
            {
                Sender = v.FindViewById<TextView>(Resource.Id.text_message_name);
                Time = v.FindViewById<TextView>(Resource.Id.text_message_time);
                Content = v.FindViewById<TextView>(Resource.Id.text_message_body);
            }
        }

        private class SentMessageHolder : RecyclerView.ViewHolder
        {
            public TextView Content { get; set; }
            public TextView Time { get; set; }

            public SentMessageHolder(View v) : base(v)
            {
                Content = v.FindViewById<TextView>(Resource.Id.text_message_body);
                Time = v.FindViewById<TextView>(Resource.Id.text_message_time);
            }
        }
    }
}