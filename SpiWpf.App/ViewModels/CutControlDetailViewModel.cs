using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Models;
using SpiWpf.Wpf.Helpers;
using SpiWpf.Wpf.Views;
using System.Globalization;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class CutControlDetailViewModel : ObservableObject
    {

        private static ContractCutAPI? DatoPrueba = new ContractCutAPI();

        private ContractCutAPI? _ContractCut;
        public ContractCutAPI? ContractCut
        {
            get { return _ContractCut; }
            set
            {
                SetProperty(ref _ContractCut, value);
            }
        }

        private List<ContractToCutCls>? _ListaParaCorte;
        public List<ContractToCutCls>? ListaParaCorte
        {
            get { return _ListaParaCorte; }
            set
            {
                SetProperty(ref _ListaParaCorte, value);
            }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public CutControlDetailViewModel()
        {
            ContractCut = new ContractCutAPI();
            ContractCut = DatoPrueba;
            ListaParaCorte = new List<ContractToCutCls>();
        }

        [RelayCommand]
        public void VolverBoton()
        {
            var mainWindow = Application.Current.MainWindow as MainPage;
            var viewModel = mainWindow!.DataContext as MainViewModel;
            viewModel!.LoadCutControl();
        }

        public void PaseParametro(ContractCutAPI value)
        {
            ContractCut = value;
            DatoPrueba = value;
        }

        [RelayCommand]
        public async Task DoGeneralCut()
        {
            IsLoading = true;

            //Vamos a verificar si la empresa usa HotSpot para hacer suspension en Mikrotik
            var responseHttp = await Repository.Get<bool>("/api/cutcontrol/checkHotSpot");
            if (responseHttp.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var UsaHotSpot = responseHttp.Response;
            if (UsaHotSpot == false)
            {
                MessageBox.Show($"Existen Contratos Activos sin IpBinding o la Empresa no Esta Configurada para Hacer Cortes a Mikrotik", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //Terminada el Check Usa HotSpot para cortes en Mikrotik

            //Recibimos La lista de Clientes a Suspender
            var responseHttp2 = await Repository.Get<List<ContractToCutCls>>("/api/cutcontrol/checkHotSpot");
            if (responseHttp2.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp2.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ListaParaCorte = responseHttp2.Response;
            //organizamos la lista por servidores, para poder hacer un corte mas rapido y organizado

            //vamos a buscar el primer y ultimo dia del mes, para buscar en CxC solo saldos del mes actual
            // porque pueden quedar Nota Cobros pendientes de meses anteriores.
            CultureInfo cult = new CultureInfo("es-ES", false);
            DateTime hoy = DateTime.Now;
            DateTime unoMes = (DateTime)ContractCut!.DateStr!;
            DateTime UltimoMes = (DateTime)ContractCut!.DateEnd!;
            string MesCurrent = DateTime.Now.ToString("MMMM", cult).ToUpper();
            var YearActual = Convert.ToInt32(DateTime.Now.Year);


            var ListServer = ListaParaCorte.DistinctBy(x => x.ipservidor);

            foreach (var itemServ in ListServer)
            {
                var datoServer = ListaParaCorte.Where(x => x.idServer == itemServ.idServer).ToList();
                MK mikrotik = new MK(itemServ.ipservidor, itemServ.puerto);
                if (!mikrotik.Login(itemServ.us, itemServ.pss))
                {
                    IsLoading = false;
                    MessageBox.Show($"Error en la Conexion al Servidor Mikrotik", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var item in datoServer)
                {
                    //Verificamos si el Contrato tiene alguna exoneracion
                    var responseHttp3 = await Repository.Get<bool>($"/api/preexonerade/exonerados/{item.idContrato}/{YearActual}/{MesCurrent}");
                    if (responseHttp3.Error)
                    {
                        IsLoading = false;
                        var msgerror = await responseHttp3.GetErrorMessageAsync();
                        MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var existeExonerados = responseHttp3.Response;
                    if (existeExonerados == true)
                    {
                        continue;
                    }
                    //var updatePreExonerated = await _context.PreExonerateds.FirstOrDefaultAsync(x => x.ContractId == item.idContrato && x.YearNumber == YearActual
                    //            && x.MonthType == MesCurrent);

                    //if (updatePreExonerated != null)
                    //{
                    //    continue;
                    //}

                    mikrotik.Send("/ip/hotspot/ip-binding/set");
                    mikrotik.Send("=.id=" + item.idIpBinding);
                    mikrotik.Send("=type=" + "regular");
                    mikrotik.Send("/ip/hotspot/ip-binding/print", true);

                    int total = 0;
                    int rest = 0;
                    string idmk;

                    foreach (var item2 in mikrotik.Read())
                    {
                        idmk = item2;
                        total = idmk.Length;
                        rest = total - 10;
                    }

                    //Actualizamos el estatus en Contrato de Activos a Suspendido
                    //var responseHttp4 = await Repository.Get<bool>($"/api/contracts/updateContrac/{item.idContrato}");
                    //if (responseHttp4.Error)
                    //{
                    //    IsLoading = false;
                    //    var msgerror = await responseHttp4.GetErrorMessageAsync();
                    //    MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //    return;
                    //}

                    //var updateContract = await _context.Contracts.FindAsync(item.idContrato);
                    //updateContract!.StateType = StateType.Suspendido;
                    //_context.Contracts.Update(updateContract);
                    //await _context.SaveChangesAsync();

                    ContractCutDetail nuevodetalle = new()
                    {
                        ContractCutId = (int)ContractCut.ContractCutId!,
                        DateSuspended = DateTime.Now,
                        ClientId = item.idCliente,
                        ContractId = item.idContrato,
                        PlanName = item.NamePlan,
                        MontoPlan = item.ValorPlan,
                        CorporateId = item.idCorporate,
                    };

                    //Vamos a mandar un nuevo registro de ContractCutDetails
                    var responseHttp5 = await Repository.Post<ContractCutDetail>($"/api/cutcontrol/toNewCutDetails", nuevodetalle);
                    if (responseHttp5.Error)
                    {
                        IsLoading = false;
                        var msgerror = await responseHttp5.GetErrorMessageAsync();
                        MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    //_context.Add(nuevodetalle);
                    //await _context.SaveChangesAsync();


                    //Actualizamos el estatus en ContractBind para que aparezca suspendido
                    var responseHttp6 = await Repository.Post<ContractCutDetail>($"/api/cutcontrol/toNewCutDetails", nuevodetalle);
                    if (responseHttp6.Error)
                    {
                        IsLoading = false;
                        var msgerror = await responseHttp6.GetErrorMessageAsync();
                        MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //Actualizamos el estatus en ContractBind para que aparezca suspendido
                    //var updateContractBind = await _context.ContractBinds.FirstOrDefaultAsync(x => x.ContractId == item.idContrato);
                    //updateContractBind!.HotSpotTypeId = 3;
                    //_context.ContractBinds.Update(updateContractBind);
                    //await _context.SaveChangesAsync();

                    SuspendeCliente suspenderCliente = new()
                    {
                        ClientId = item.idCliente,
                        ContractId = item.idContrato,
                        Motivo = "Falta de Pago Oportuno Mes" + " " + DateTime.Now.Month + " - " + DateTime.Now.Year,
                        PlanName = item.NamePlan,
                        MontoPlan = item.ValorPlan
                    };

                    var responseHttp7 = await Repository.Post<SuspendeCliente>($"/api/suspended/CreateSuspended/", suspenderCliente);
                    if (responseHttp7.Error)
                    {
                        IsLoading = false;
                        var msgerror = await responseHttp7.GetErrorMessageAsync();
                        MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //Mandamos los cliente a suspension
                    //var CheckSuspended = await _context.ContractSuspendeds.FirstOrDefaultAsync(x => x.ContractId == item.idContrato && x.ClientId == item.idCliente);
                    //if (CheckSuspended == null)
                    //{
                    //    ContractSuspended modelo = new()
                    //    {
                    //        CorporateId = item.idCorporate,
                    //        DateSuspended = DateTime.Now,
                    //        ClientId = item.idCliente,
                    //        ContractId = item.idContrato,
                    //        Motivo = "Falta de Pago Oportuno Mes" + " " + DateTime.Now.Month + " - " + DateTime.Now.Year,
                    //        PlanName = item.NamePlan,
                    //        MontoPlan = item.ValorPlan
                    //    };
                    //    _context.ContractSuspendeds.Add(modelo);
                    //    await _context.SaveChangesAsync();
                    //}
                    //else
                    //{
                    //    continue;
                    //}
                }
                mikrotik.Close();
            }

            var responseHttp8 = await Repository.Get<bool>($"/api/cutcontrol/upContractCut/{ContractCut.ContractCutId}");
            if (responseHttp8.Error)
            {
                IsLoading = false;
                var msgerror = await responseHttp8.GetErrorMessageAsync();
                MessageBox.Show($"{msgerror}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //var UpContractCut = await _context.ContractCuts.FirstOrDefaultAsync(c => c.ContractCutId == id);
            //UpContractCut!.Creado = true;
            //UpContractCut.DateCreado = DateTime.Now;
            //_context.ContractCuts.Update(UpContractCut);
            //await _context.SaveChangesAsync();

        }
    }
}
