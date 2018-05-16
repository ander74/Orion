#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.DataModels;
using Orion.Models;
using Orion.Servicios;
using Orion.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Orion.ViewModels {

	public partial class VentanaNuevoGrupoVM :NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private IMensajes mensajes;

		#endregion


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================
		public VentanaNuevoGrupoVM(IMensajes servicioMensajes) {
			mensajes = servicioMensajes;
		}

		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private ObservableCollection<GrupoGraficos> _listagrupos;
		public ObservableCollection<GrupoGraficos> ListaGrupos {
			get { return _listagrupos; }
			set {
				if (_listagrupos != value) {
					_listagrupos = value;
					PropiedadCambiada();
				}
			}
		}

		private DateTime _fechaactual;
		public DateTime FechaActual {
			get { return _fechaactual; }
			set {
				if (_fechaactual != value) {
					_fechaactual = value;
					PropiedadCambiada();
				}
			}
		}


		private GrupoGraficos _gruposeleccionado;
		public GrupoGraficos GrupoSeleccionado {
			get { return _gruposeleccionado; }
			set {
				if (_gruposeleccionado != value) {
					_gruposeleccionado = value;
				}
			}
		}


		private string _archivoword = "";
		public string ArchivoWord {
			get { return _archivoword; }
			set {
				if (_archivoword != value) {
					_archivoword = value;
					PropiedadCambiada();
				}
			}
		}


		private string _notas = "";
		public string Notas {
			get { return _notas; }
			set {
				if (_notas != value) {
					_notas = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _nuevomarcado;
		public bool NuevoMarcado {
			get { return _nuevomarcado; }
			set {
				if (_nuevomarcado != value) {
					_nuevomarcado = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _repetirmarcado;
		public bool RepetirMarcado {
			get { return _repetirmarcado; }
			set {
				if (_repetirmarcado != value) {
					_repetirmarcado = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _wordmarcado;
		public bool WordMarcado {
			get { return _wordmarcado; }
			set {
				if (_wordmarcado != value) {
					_wordmarcado = value;
					PropiedadCambiada();
				}
			}
		}

		#endregion


		// ====================================================================================================
		#region COMANDOS
		// ====================================================================================================

		#region CANCELAR
		//Comando
		public ICommand cmdCancelar {
			get { return new RelayCommand(p => Cancelar(p)); }
		}

		// Ejecución del comando
		private void Cancelar(object parametro) {
			if (parametro == null) return;
			((Window)parametro).Close();
		}
		#endregion


		#region ACEPTAR
		//Comando
		public ICommand cmdAceptar {
			get { return new RelayCommand(p => Aceptar(p), p => PuedeAceptar()); }
		}

		// Se puede ejecutar
		private bool PuedeAceptar() {
			if (!NuevoMarcado && !RepetirMarcado && !WordMarcado) return false;
			return true;
		}

		// Ejecución del comando
		private void Aceptar(object parametro) {
			if (parametro == null) return;
			Window ventana = (Window)parametro;
			try {
				// Si la fecha ya existe, mostramos mensaje
				if (BdGruposGraficos.ExisteGrupo(FechaActual)) {
					mensajes.VerMensaje("Ya existe un grupo con la fecha elegida.", "ERROR");
					return;
				}
				if (NuevoMarcado) {
					if (String.IsNullOrEmpty(Notas.Trim())) Notas = FechaActual.ToString("dd-MM-yyyy");
					BdGruposGraficos.NuevoGrupo(FechaActual, Notas);
					ventana.DialogResult = true;
					ventana.Close();
				}
				if (RepetirMarcado) { //TODO: Sustituir por grupo seleccionado.
					ObservableCollection<Grafico> graficos = BdGraficos.getGraficosGrupoPorFecha(GrupoSeleccionado.Validez);
					ObservableCollection<ValoracionGrafico> valoraciones = new ObservableCollection<ValoracionGrafico>();
					if (String.IsNullOrEmpty(Notas.Trim())) Notas = FechaActual.ToString("dd-MM-yyyy");
					int idgruponuevo = BdGruposGraficos.NuevoGrupo(FechaActual, Notas);
					int idgraficonuevo = -1;
					foreach (Grafico grafico in graficos) {
						grafico.IdGrupo = idgruponuevo;
						idgraficonuevo = BdGraficos.InsertarGrafico(grafico);
						valoraciones = BdValoracionesGraficos.getValoraciones(grafico.Id);
						foreach (ValoracionGrafico valoracion in valoraciones) {
							valoracion.IdGrafico = idgraficonuevo;
							BdValoracionesGraficos.InsertarValoracion(valoracion);
						}
					}
					// Cerramos la ventana enviando True.
					ventana.DialogResult = true;
					ventana.Close();
				}
				if (WordMarcado) {
					if (String.IsNullOrEmpty(ArchivoWord)) {
						mensajes.VerMensaje("No ha seleccionado ningún archivo.", "ERROR");
						return;
					}

					CrearGrupoDeWord();

					// Cerramos la ventana enviando True.
					ventana.DialogResult = true;
					ventana.Close();
				}

			} catch (Exception ex) {
				mensajes.VerError("VentanaNuevoGrupoVM.Aceptar", ex);
			}
		}

		#endregion


		#region SELECCIONAR ARCHIVO
		//Comando
		public ICommand cmdSeleccionarArchivo {
			get { return new RelayCommand(p => SeleccionarArchivo()); }
		}

		// Ejecución del comando
		private void SeleccionarArchivo() {
			Microsoft.Win32.OpenFileDialog dialogo = new Microsoft.Win32.OpenFileDialog();

			dialogo.DefaultExt = ".docx";
			dialogo.Filter = "Documentos de Word|*.docx;*.doc;*.rtf|Archivos de texto|*.txt";
			dialogo.Title = "Abrir archivo de gráficos";
			dialogo.CheckFileExists = true;
			dialogo.Multiselect = false;
			bool? resultado = dialogo.ShowDialog();
			if (resultado == true) {
				ArchivoWord = dialogo.FileName;
			}
		}
		#endregion


		#endregion


		// ====================================================================================================
		#region  MÉTODOS AUXILIARES
		// ====================================================================================================

		private void CrearGrupoDeWord2() {

			// Si el archivo no existe, salimos.
			if (!File.Exists(ArchivoWord)) return;

			// Creamos la aplicación de Word.
			Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
			Microsoft.Office.Interop.Word.Document wordDoc = null;
			wordApp.Visible = false;

            try {
                // Abrimos el documento de word.
                wordDoc = wordApp.Documents.Open(ArchivoWord);
                
                // Añadimos un retorno de carro delante de las valoraciones.
                wordDoc.Content.Find.Execute("Balorazioa", false, true, false, false, false, true, 1, false, "\rBalorazioa", 2, false, false, false, false);

                // Creamos el grupo nuevo
                if (String.IsNullOrEmpty(Notas.Trim())) Notas = FechaActual.ToString("dd-MM-yyyy");
                int idgruponuevo = BdGruposGraficos.NuevoGrupo(FechaActual, Notas);
                // Definimos las variables a usar
                bool EnUnGrafico = false;
                Grafico grafico = new Grafico();
                ValoracionGrafico valoracionanterior = new ValoracionGrafico();
                bool IniciaGrafico = false;
                bool SalirDelBucle = false;

                // Recorremos los párrafos del documento.
                foreach (Microsoft.Office.Interop.Word.Paragraph parrafo in wordDoc.Paragraphs) {
                
                    ValoracionGrafico valoracion = new ValoracionGrafico();
                    string texto = GestionGraficos.LimpiarTexto(parrafo.Range.Text);
                    GestionGraficos.TipoValoracion tipo = GestionGraficos.ParseaTexto(texto, ref valoracion);

                    switch (tipo) {
                        case GestionGraficos.TipoValoracion.InicioGrafico:
                            if (EnUnGrafico) {
                                // Gestionamos el error
                                if (VerErrorGrafico(grafico.Numero, parrafo.Range.Text, texto)) {
                                    grafico = new Grafico();
                                    grafico.IdGrupo = idgruponuevo;
                                    grafico.Numero = (int)valoracion.Linea;
                                    IniciaGrafico = true;
                                    EnUnGrafico = true;
                                    continue;
                                } else {
                                    SalirDelBucle = true;
                                }
                            } else {
                                grafico = new Grafico();
                                grafico.IdGrupo = idgruponuevo;
                                grafico.Numero = (int)valoracion.Linea;
                                if (grafico.Numero % 2 == 0) grafico.Turno = 2;
                                IniciaGrafico = true;
                                EnUnGrafico = true;
                            }
                            break;
                        case GestionGraficos.TipoValoracion.FinalGrafico:
                            if (!EnUnGrafico) {
                                // Gestionamos el error
                                if (VerErrorGrafico(grafico.Numero, parrafo.Range.Text, texto)) {
                                    EnUnGrafico = false;
                                    IniciaGrafico = false;
                                    continue;
                                } else {
                                    SalirDelBucle = true;
                                }
                            } else {
                                grafico.Final = valoracionanterior.Inicio;
                                grafico.Valoracion = valoracion.Tiempo;
                                grafico.Recalcular();
                                BdGraficos.InsertarGrafico(grafico);
                                IniciaGrafico = false;
                                EnUnGrafico = false;
                            }
                            break;
                        case GestionGraficos.TipoValoracion.Completo:
                        case GestionGraficos.TipoValoracion.Parcial:
                        case GestionGraficos.TipoValoracion.ParcialCodigo:
                        case GestionGraficos.TipoValoracion.ParcialLinea:
                        case GestionGraficos.TipoValoracion.ParcialVacio:
                            if (EnUnGrafico) {
                                if (IniciaGrafico) {
                                    grafico.Inicio = valoracion.Inicio;
                                    IniciaGrafico = false;
                                } else {
                                    valoracionanterior.Final = valoracion.Inicio;
                                }
                                grafico.ListaValoraciones.Add(valoracion);
                                valoracionanterior = valoracion;
                            }
                            break;
                        case GestionGraficos.TipoValoracion.Informacion:
                            if (EnUnGrafico) {
                                if (!IniciaGrafico) {
                                    valoracion.Inicio = valoracionanterior.Inicio;
                                }
                                grafico.ListaValoraciones.Add(valoracion);
                            }
                            break;
                    }
                    if (SalirDelBucle) break;
                }

            } catch (Exception ex) {
                mensajes.VerError("VentanaNuevoGrupoVM.CrearGrupoDeWord", ex);
                return;
            } finally {
                if (wordDoc != null) wordDoc.Close(false);
                if (wordApp != null) wordApp.Quit(false);
            }
		}


        private void CrearGrupoDeWord() {

            // Si el archivo no existe, salimos.
            if (!File.Exists(ArchivoWord)) return;

            // Creamos la aplicación de Word.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wordDoc = null;
            wordApp.Visible = false;

            try {
                // Abrimos el documento de word.
                wordDoc = wordApp.Documents.Open(ArchivoWord);

                StringBuilder sb = new StringBuilder();

                foreach (Microsoft.Office.Interop.Word.Paragraph parrafo in wordDoc.Paragraphs) {
                    string t = parrafo.Range.Text + "\n";
                    //t.Replace("\r", "\n");
                    t = t.Replace("Balorazioa", "Final\nBalorazioa");
                    t = t.Replace("0h", "0");
                    t = t.Replace("5h", "5");
                    sb.Append(t);
                }
                string[] todo = sb.ToString().Split('\n');


                // Creamos el grupo nuevo
                if (String.IsNullOrEmpty(Notas.Trim())) Notas = FechaActual.ToString("dd-MM-yyyy");
                int idgruponuevo = BdGruposGraficos.NuevoGrupo(FechaActual, Notas);
                // Definimos las variables a usar
                bool EnUnGrafico = false;
                Grafico grafico = new Grafico();
                ValoracionGrafico valoracionanterior = new ValoracionGrafico();
                bool IniciaGrafico = false;
                bool SalirDelBucle = false;
                
                // Recorremos los párrafos del documento.
                foreach (string parrafo in todo) {

                    ValoracionGrafico valoracion = new ValoracionGrafico();
                    string texto = GestionGraficos.LimpiarTexto(parrafo);
                    GestionGraficos.TipoValoracion tipo = GestionGraficos.ParseaTexto(texto, ref valoracion);

                    switch (tipo) {
                        case GestionGraficos.TipoValoracion.InicioGrafico:
                            if (EnUnGrafico) {
                                // Gestionamos el error
                                if (VerErrorGrafico(grafico.Numero, parrafo, texto)) {
                                    grafico = new Grafico();
                                    grafico.IdGrupo = idgruponuevo;
                                    grafico.Numero = (int)valoracion.Linea;
                                    IniciaGrafico = true;
                                    EnUnGrafico = true;
                                    continue;
                                } else {
                                    SalirDelBucle = true;
                                }
                            } else {
                                grafico = new Grafico();
                                grafico.IdGrupo = idgruponuevo;
                                grafico.Numero = (int)valoracion.Linea;
                                if (grafico.Numero % 2 == 0) grafico.Turno = 2;
                                IniciaGrafico = true;
                                EnUnGrafico = true;
                            }
                            break;
                        case GestionGraficos.TipoValoracion.FinalGrafico:
                            if (!EnUnGrafico) {
                                // Gestionamos el error
                                if (VerErrorGrafico(grafico.Numero, parrafo, texto)) {
                                    EnUnGrafico = false;
                                    IniciaGrafico = false;
                                    continue;
                                } else {
                                    SalirDelBucle = true;
                                }
                            } else {
                                grafico.Final = valoracionanterior.Inicio;
                                grafico.Valoracion = valoracion.Tiempo;
                                grafico.Recalcular();
                                BdGraficos.InsertarGrafico(grafico);
                                IniciaGrafico = false;
                                EnUnGrafico = false;
                            }
                            break;
                        case GestionGraficos.TipoValoracion.Completo:
                        case GestionGraficos.TipoValoracion.Parcial:
                        case GestionGraficos.TipoValoracion.ParcialCodigo:
                        case GestionGraficos.TipoValoracion.ParcialLinea:
                        case GestionGraficos.TipoValoracion.ParcialVacio:
                            if (EnUnGrafico) {
                                if (IniciaGrafico) {
                                    grafico.Inicio = valoracion.Inicio;
                                    IniciaGrafico = false;
                                } else {
                                    valoracionanterior.Final = valoracion.Inicio;
                                }
                                grafico.ListaValoraciones.Add(valoracion);
                                valoracionanterior = valoracion;
                            }
                            break;
                        case GestionGraficos.TipoValoracion.Informacion:
                            if (EnUnGrafico) {
                                if (!IniciaGrafico) {
                                    valoracion.Inicio = valoracionanterior.Inicio;
                                }
                                grafico.ListaValoraciones.Add(valoracion);
                            }
                            break;
                    }
                    if (SalirDelBucle) break;
                }

            } catch (Exception ex) {
                mensajes.VerError("VentanaNuevoGrupoVM.CrearGrupoDeWord", ex);
                return;
            } finally {
                if (wordDoc != null) wordDoc.Close(false);
                if (wordApp != null) wordApp.Quit(false);
            }
        }
        

        private bool VerErrorGrafico(decimal numero, string textoOriginal, string texto) {
			bool? resultado = mensajes.VerMensaje($"Se ha producido un error cerca del gráfico {numero.ToString("0000")}\n" +
														  "¿Desea continuar procesando?\n\n" + textoOriginal + "\n\n" + texto,
														  "Error en gráfico",
														  true);
			return resultado == true;
		}





		#endregion





	} // Fin de clase.
}
