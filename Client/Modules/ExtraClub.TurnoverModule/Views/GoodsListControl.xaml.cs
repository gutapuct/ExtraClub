using ExtraClub.ServiceModel;
using ExtraClub.TurnoverModule.ViewModels;

namespace ExtraClub.TurnoverModule.Views
{
    public partial class GoodsListControl
    {
        TurnoverLargeViewModel _model;

        public GoodsListControl(TurnoverLargeViewModel model)
        {
            InitializeComponent();
            DataContext = _model = model;
            
            DictControl.Init(DictionaryManager, ClientContext);
            //if (!ClientContext.CheckPermission("DisableCentral"))
            {
                DictControl.RegisterDictionary("ProductTypes");
                DictControl.RegisterDictionary("GoodsCategories");
            }
            DictControl.RegisterDictionary("Manufacturers");

        }
    }
}
