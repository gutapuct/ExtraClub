using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class Permission
    {
        private bool helper = false;
        public bool Helper
        {
            get
            {
                return helper;
            }
            set
            {
                if (helper != value)
                {
                    helper = value;
                    //if (Permissions != null) Permissions.ForEach(i => i.Helper = value);
                    if (Parent != null && value) Parent.Helper = true;//.CheckHelper();
                    OnPropertyChanged("Helper");
                }
            }
        }

        partial void OnDeserialized()
        {
            helper = false;
        }

        private void CheckHelper()
        {
            //if (Permissions.Count(i => i.Helper/*.HasValue && i.Helper.Value*/) == Permissions.Count)
            //{
            //    helper = true;
            //    OnPropertyChanged("Helper");
            //}
            //else if (Permissions.Count(i => /*i.Helper.HasValue && */!i.Helper/*.Value*/) == Permissions.Count)
            //{
            //    helper = false;
            //    OnPropertyChanged("Helper");
            //}
            //else
            //{
            //    helper = null;
            //    OnPropertyChanged("Helper");
            //}
        }

        public Permission Parent { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
