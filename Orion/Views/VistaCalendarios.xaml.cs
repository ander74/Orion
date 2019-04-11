#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Views {

	using System.Windows;
	using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

	public partial class VistaCalendarios : UserControl {
		public VistaCalendarios() {

			InitializeComponent();
		}


		// ====================================================================================================
		#region GESTIÓN BOTONES DESPLEGABLES
		// ====================================================================================================

		// AL PULSAR UNO DE LOS BOTONES DEL DESPLEGABLE FILTRAR
		private void BotonFiltrado_Click(object sender, RoutedEventArgs e) {
			BtDropFiltros.IsOpen = false;
		}


		// AL PULSAR UNO DE LOS BOTONES DEL DESPLEGABLE ACCIONES
		private void BtAccionesLotes_Click(object sender, RoutedEventArgs e) {
			if (PanelAccionesLotes.Visibility == Visibility.Collapsed) {
				BordePanelAccionesLotes.Visibility = Visibility.Visible;
				PanelAccionesLotes.Visibility = Visibility.Visible;
				BtCerrarAccionesLotes.Visibility = Visibility.Visible;
			} else {
				BordePanelAccionesLotes.Visibility = Visibility.Collapsed;
				PanelAccionesLotes.Visibility = Visibility.Collapsed;
				BtCerrarAccionesLotes.Visibility = Visibility.Collapsed;
			}
			BtDropAcciones.IsOpen = false;
		}


		// AL PULSAR UNO DE LOS BOTONES DEL DESPLEGABLE CREAR PDF
		private void BtCreandoPdf_Click(object sender, RoutedEventArgs e) {
			BtDropCrearPdf.IsOpen = false;
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region GESTIÓN RESUMEN PIJAMA
		// ====================================================================================================

		private void BtMostrarTodos_Click(object sender, RoutedEventArgs e) {
			BtHorasMes.IsChecked = false;
			BtDiasMes.IsChecked = false;
			BtFindesMes.IsChecked = false;
			BtDietasMes.IsChecked = false;
			BtResumenMes.IsChecked = false;
		}

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region GESTIÓN PANEL FECHA
		// ====================================================================================================
		private void BtAbrirPanelFecha_Click(object sender, RoutedEventArgs e) {
			if (PanelFecha.Visibility == Visibility.Collapsed) {
				PanelFecha.Visibility = Visibility.Visible;
			} else {
				PanelFecha.Visibility = Visibility.Collapsed;
			}
		}

		private void QCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
			PanelFecha.Visibility = Visibility.Collapsed;
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region GESTIÓN CALENDARIO-PIJAMA
		// ====================================================================================================

		private void BtAbrirPijama_Click(object sender, RoutedEventArgs e) {
			// OCULTAR
			BotonesCalendarios.Visibility = Visibility.Collapsed;
			PanelAccionesPorLotes.Visibility = Visibility.Collapsed;
			TablaCalendarios.Visibility = Visibility.Collapsed;
			// MOSTRAR
			BotonesPijama.Visibility = Visibility.Visible;
			PanelConductorPijama.Visibility = Visibility.Visible;
			TablaPijama.Visibility = Visibility.Visible;
			PanelResumen.Visibility = Visibility.Visible;
			BordePanelResumen.Visibility = Visibility.Visible;
		}


		private void TablaCalendarios_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			// OCULTAR
			BotonesCalendarios.Visibility = Visibility.Collapsed;
			PanelAccionesPorLotes.Visibility = Visibility.Collapsed;
			TablaCalendarios.Visibility = Visibility.Collapsed;
			// MOSTRAR
			BotonesPijama.Visibility = Visibility.Visible;
			PanelConductorPijama.Visibility = Visibility.Visible;
			TablaPijama.Visibility = Visibility.Visible;
			PanelResumen.Visibility = Visibility.Visible;
			BordePanelResumen.Visibility = Visibility.Visible;

			e.Handled = true;
		}


		private void BtCerrarPijama_Click(object sender, RoutedEventArgs e) {
			// OCULTAR
			BotonesPijama.Visibility = Visibility.Collapsed;
			PanelConductorPijama.Visibility = Visibility.Collapsed;
			TablaPijama.Visibility = Visibility.Collapsed;
			PanelResumen.Visibility = Visibility.Collapsed;
			BordePanelResumen.Visibility = Visibility.Collapsed;
			// MOSTRAR
			BotonesCalendarios.Visibility = Visibility.Visible;
			PanelAccionesPorLotes.Visibility = Visibility.Visible;
			TablaCalendarios.Visibility = Visibility.Visible;
		}

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BOTÓN QUITAR FILTRO
        // ====================================================================================================

        private void BtQuitarFiltro_Click(object sender, RoutedEventArgs e) {
            var view = CollectionViewSource.GetDefaultView(TablaCalendarios.ItemsSource);
            if (view != null && view.SortDescriptions != null) {
                view.SortDescriptions.Clear();
                view.Filter = null;
            }
            TablaCalendarios.Columns[0].SortDirection = null;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region GESTION PANEL RESUMEN Y GRÁFICOS
        // ====================================================================================================

        private void BtPanelPijama_Click(object sender, RoutedEventArgs e) {
            PanelGraficosPijama.Visibility = Visibility.Collapsed;
            PanelPijama.Visibility = Visibility.Visible;
        }


        private void BtPanelGraficosResumen_Click(object sender, RoutedEventArgs e) {
            PanelPijama.Visibility = Visibility.Collapsed;
            PanelGraficosPijama.Visibility = Visibility.Visible;
        }


        #endregion
        // ====================================================================================================

    }
}
