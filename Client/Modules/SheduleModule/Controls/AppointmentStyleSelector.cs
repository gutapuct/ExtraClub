using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace ExtraClub.ScheduleModule.Controls
{
	public class AppointmentStyleSelector : OrientedAppointmentItemStyleSelector
	{
		private Style l0HorizontalStyle;
		private Style l1HorizontalStyle;
		private Style l2HorizontalStyle;
		private Style l3HorizontalStyle;

		public Style L0HorizontalStyle
		{
			get
			{
				return this.l0HorizontalStyle;
			}
			set
			{
				this.l0HorizontalStyle = value;
			}
		}

		public Style L1HorizontalStyle
		{
			get
			{
				return this.l1HorizontalStyle;
			}
			set
			{
				this.l1HorizontalStyle = value;
			}
		}

		public Style L2HorizontalStyle
		{
			get
			{
				return this.l2HorizontalStyle;
			}
			set
			{
				this.l2HorizontalStyle = value;
			}
		}

		public Style L3HorizontalStyle
		{
			get
			{
				return this.l3HorizontalStyle;
			}
			set
			{
				this.l3HorizontalStyle = value;
			}
		}

		public override Style SelectStyle(object item, DependencyObject container, ViewDefinitionBase activeViewDefinition)
		{
			IAppointment appointment = item as IAppointment;
			if (appointment == null)
			{
				return base.SelectStyle(item, container, activeViewDefinition);
			}

			IResource resource = appointment.Resources.OfType<IResource>().FirstOrDefault((IResource r) => r.ResourceType == "Status");
            if (resource != null)
            {

                switch (resource.ResourceName)
                {
                    case "0": return this.L0HorizontalStyle;
                    case "1": return this.L1HorizontalStyle;
                    case "2": return this.L2HorizontalStyle;
                    case "3": return this.L3HorizontalStyle;
                    default: break;
                }
            }

			return base.SelectStyle(item, container, activeViewDefinition);
		}
	}
}