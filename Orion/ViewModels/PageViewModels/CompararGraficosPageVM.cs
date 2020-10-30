#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion


using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Orion.Models;

namespace Orion.ViewModels.PageViewModels {

    public class CompararGraficosPageVM : PageViewModelBase {

        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static CompararGraficosPageVM instance;

        public static CompararGraficosPageVM GetInstance() {
            return instance ?? new CompararGraficosPageVM();
        }

        private CompararGraficosPageVM() {
            instance = this;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        // Mapeado a los viewmodels que necesitemos usar.
        // ----------------------------------------------
        public GraficosViewModel GraficosVM => App.Global.GraficosVM;



        private GrupoGraficos grupoSeleccionado1;
        public GrupoGraficos GrupoSeleccionado1 {
            get => grupoSeleccionado1;
            set => SetValue(ref grupoSeleccionado1, value);
        }


        private GrupoGraficos grupoSeleccionado2;
        public GrupoGraficos GrupoSeleccionado2 {
            get => grupoSeleccionado2;
            set => SetValue(ref grupoSeleccionado2, value);
        }


        private List<object> graficosComparados;
        public List<object> GraficosComparados {
            get => graficosComparados;
            set => SetValue(ref graficosComparados, value);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================

        #region COMPARAR
        // Comando
        private ICommand cmdComparar;
        public ICommand CmdComparar {
            get {
                if (cmdComparar == null) cmdComparar = new RelayCommand(p => Comparar(), p => PuedeComparar());
                return cmdComparar;
            }
        }
        // Puede
        private bool PuedeComparar() {
            return GrupoSeleccionado1 != null && GrupoSeleccionado2 != null && GrupoSeleccionado1 != GrupoSeleccionado2;
        }
        // Ejecución del comando
        private void Comparar() {
            try {
                var lista1 = App.Global.GetRepository(App.Global.CentroActual).GetGraficos(GrupoSeleccionado1.Validez);
                var lista2 = App.Global.GetRepository(App.Global.CentroActual).GetGraficos(GrupoSeleccionado2.Validez);
                if (lista1 == null || lista2 == null) return;
                var graficos = lista1.Select(g => g.Numero).Union(lista2.Select(gg => gg.Numero)).OrderBy(n => n);
                if (graficos == null || graficos.Count() == 0) return;
                var lista = new List<object>();
                foreach (var numero in graficos) {
                    //int numero = grafico;
                    string diferencias = string.Empty;
                    // Comprobamos que el gráfico esté en las dos listas.
                    var g1 = lista1.FirstOrDefault(g => g.Numero == numero);
                    var g2 = lista2.FirstOrDefault(g => g.Numero == numero);
                    if (g1 == null) diferencias += $"No está en grupo <{GrupoSeleccionado1.Validez:dd-MM-yyyy}>.\n";
                    if (g2 == null) diferencias += $"No está en grupo <{GrupoSeleccionado2.Validez:dd-MM-yyyy}>.\n";
                    // Si está en los dos grupos, se compara.
                    if (g1 != null && g2 != null) {
                        if (g1.Turno != g2.Turno) diferencias += $"Turno diferente.\n";
                        if (g1.Inicio != g2.Inicio) diferencias += $"Inicio diferente.\n";
                        if (g1.Final != g2.Final) diferencias += $"Final diferente.\n";
                        if (g1.InicioPartido != g2.InicioPartido) diferencias += $"Inicio partido diferente.\n";
                        if (g1.FinalPartido != g2.FinalPartido) diferencias += $"Final partido diferente.\n";
                        if (g1.Valoracion != g2.Valoracion) diferencias += $"Valoración diferente.\n";
                    }
                    if (!string.IsNullOrEmpty(diferencias)) {
                        diferencias = diferencias.Substring(0, diferencias.Length - 1);
                        lista.Add(new { Numero = numero, Diferencias = diferencias });
                    }
                }
                GraficosComparados = lista;

            } catch (System.Exception ex) {
                App.Global.Mensajes.VerError("CompararGraficosPageVM.Comparar", ex);
                return;
            }
        }

        // Clase Auxiliar con el comparador usado para evitar duplicados.

        #endregion

        #endregion
        // ====================================================================================================


    }
}
