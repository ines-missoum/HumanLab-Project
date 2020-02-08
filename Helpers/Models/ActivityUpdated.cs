using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using humanlab.ViewModels;
using System.Diagnostics;
using humanlab.DAL;

namespace humanlab.Helpers.Models
{
    public class ActivityUpdated: BaseViewModel
    {

        public Activity Activity { get; set; }
        public bool editMode;
        private ActivityRepository activityRepository;

        public DelegateCommand<object> DeleteActivityDelegate { get; set; }
        public DelegateCommand test { get; set; }
        public DelegateCommand<object> updateActivityDelegate { get; set; }


        public ActivityUpdated(Activity newActivity, DelegateCommand<object> removeActivityFromList, DelegateCommand<object> updateActivityDelegate)
        {
            Activity = newActivity;
            EditMode = false;
            activityRepository = new ActivityRepository();
            DeleteActivityDelegate = removeActivityFromList;
            this.updateActivityDelegate = updateActivityDelegate;

        }
        public bool EditMode
        {
            get => editMode;
            set => SetProperty(ref editMode, value, "EditMode");
        }
    }

}
