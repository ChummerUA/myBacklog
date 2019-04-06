using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace myBacklog.Behaviors
{
    class StateEntryBehavior: Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Completed += Bindable_Completed;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.Completed -= Bindable_Completed;
        }

        private void Bindable_Completed(object sender, EventArgs e)
        {
            (sender as Entry).Text = "";
        }
    }
}
