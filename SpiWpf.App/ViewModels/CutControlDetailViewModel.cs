using CommunityToolkit.Mvvm.ComponentModel;
using SpiWpf.Entities.DTOs;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class CutControlDetailViewModel : ObservableObject
    {

        private ContractCutAPI? _ContractCut;
        public ContractCutAPI? ContractCut
        {
            get { return _ContractCut; }
            set
            {
                SetProperty(ref _ContractCut, value);
            }
        }

        private int _CutControlID;
        public int CutControlID
        {
            get { return _CutControlID; }
            set
            {
                SetProperty(ref _CutControlID, value);
            }
        }

        public CutControlDetailViewModel()
        {

        }

        public void PaseParametro(ContractCutAPI value)
        {
            ContractCut = value;
        }
    }
}
