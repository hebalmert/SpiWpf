using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Enum;
using SpiWpf.Wpf.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public  partial class CutControlNewViewModel : ObservableObject
    {
        public ObservableCollection<MesDTO>? MesLst { get; set; }

        public ObservableCollection<AnoDTO>? AnoLst { get; set; }

        private MesDTO? _SelectedMes;
        public MesDTO? SelectedMes
        {
            get { return _SelectedMes; }
            set
            {
                SetProperty(ref _SelectedMes, value);
            }
        }

        private AnoDTO? _SelectedAno;
        public AnoDTO? SelectedAno
        {
            get { return _SelectedAno; }
            set
            {
                SetProperty(ref _SelectedAno, value);
            }
        }

        private DateTime? _FechaCrear;
        public DateTime? FechaCrear
        {
            get { return _FechaCrear; }
            set
            {
                SetProperty(ref _FechaCrear, value);
            }
        }

        private DateTime? _FechaInicio;
        public DateTime? FechaInicio
        {
            get { return _FechaInicio; }
            set
            {
                SetProperty(ref _FechaInicio, value);
            }
        }

        private DateTime? _FechaFinal;
        public DateTime? FechaFinal
        {
            get { return _FechaFinal; }
            set
            {
                SetProperty(ref _FechaFinal, value);
            }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private List<MesDTO>? meses = new List<MesDTO>();

        private List<AnoDTO>? anos = new List<AnoDTO>();

        public CutControlNewViewModel()
        {
            MesLst = new ObservableCollection<MesDTO>();
            AnoLst = new ObservableCollection<AnoDTO>();
            FechaCrear = new DateTime();
            FechaInicio = new DateTime();
            FechaFinal = new DateTime();
            LoadMesLista();
            LoadAnoLista();
            LoadFechaCombos();
        }

        private void LoadFechaCombos()
        {
            DateTime hoy = DateTime.Now;
            DateTime unoMes = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime UltimoMes = unoMes.AddMonths(1).AddDays(-1);
            FechaCrear = hoy;
            FechaInicio = unoMes;
            FechaFinal = UltimoMes;
        }

        private void LoadAnoLista()
        {
            var Fano = DateTime.Now.Year;
            var Fano2 = DateTime.Now.Year + 1;
            anos!.Add(new AnoDTO { Id = 0, AnoName = $"{Fano}" });
            anos!.Add(new AnoDTO { Id = 1, AnoName = $"{Fano2}" });
            AnoLst?.Clear();
            if (anos != null || anos!.Count > 0)
            {
                foreach (var item in anos!)
                {
                    AnoLst!.Add(item);
                }
            }
            SelectedAno = AnoLst!.Where(x=> x.Id == 0).FirstOrDefault();
        }

        private void LoadMesLista()
        {
            meses = Enum.GetValues(typeof(MonthType)).Cast<MonthType>().Select(x => new MesDTO
            {
                Id = (int)x,
                MesName = x.ToString()
            }).ToList();
            MesLst?.Clear();
            if (meses != null || meses!.Count > 0)
            {
                foreach (var item in meses!)
                {
                    MesLst!.Add(item);
                }
            }
            var mesactual = DateTime.Now.Month;
            SelectedMes = MesLst!.Where(x=> x.Id == mesactual).FirstOrDefault();
        }


        [RelayCommand]
        public async Task GuardarBoton() 
        {
            ContractCutAPI modelo = new()
            {
                DateCut = (DateTime)FechaCrear!,
                YearNumber = Convert.ToInt32(SelectedAno!.AnoName),
                Mes = SelectedMes!.MesName,
                DateStr = FechaInicio,
                DateEnd = FechaFinal
            };

            var responseHttp = await Repository.Post<ContractCutAPI, ContractCutAPI>("/api/cutcontrol/NewCutControlGen", modelo);
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var BackModelo = responseHttp.Response;

            IsLoading = false;


            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadCutControlDetail(BackModelo);

        }

    }
}
