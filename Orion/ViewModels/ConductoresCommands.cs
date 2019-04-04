#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels {

    using Models;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    public partial class ConductoresViewModel {


        #region BORRAR CONDUCTOR

        private ICommand _cmdborrarconductor;
        public ICommand cmdBorrarConductor {
            get {
                if (_cmdborrarconductor == null) _cmdborrarconductor = new RelayCommand(p => BorrarConductor(), p => PuedeBorrarConductor());
                return _cmdborrarconductor;
            }
        }

        private bool PuedeBorrarConductor() {
            return ConductorSeleccionado != null;
        }

        private void BorrarConductor() {
            if (ConductorSeleccionado == null) return;
            if (mensajes.VerMensaje($"Vas a borrar al conductor:\n\n{ConductorSeleccionado.Id:000}: {ConductorSeleccionado.Apellidos}\n\n" +
                                    $"Esto hará que se borren todos sus calendarios.\n\n" +
                                    $"¿Deseas continuar?", "ATENCIÓN", true) == false) return;
            _listaborrados.Add(ConductorSeleccionado);
            ListaConductores.Remove(ConductorSeleccionado);
            HayCambios = true;
        }
        #endregion


        #region DESHACER BORRAR

        private ICommand _cmddeshacerborrar;
        public ICommand cmdDeshacerBorrar {
            get {
                if (_cmddeshacerborrar == null) _cmddeshacerborrar = new RelayCommand(p => DeshacerBorrar(), p => PuedeDeshacerBorrar());
                return _cmddeshacerborrar;
            }
        }

        private bool PuedeDeshacerBorrar() {
            return _listaborrados.Count > 0;
        }

        private void DeshacerBorrar() {
            if (_listaborrados == null) return;
            foreach (Conductor conductor in _listaborrados) {
                if (conductor.Nuevo) {
                    ListaConductores.Add(conductor);
                } else {
                    ListaConductores.Add(conductor);
                    conductor.Nuevo = false;
                }
                HayCambios = true;
            }
            _listaborrados.Clear();
        }
        #endregion


        #region BORRAR REGULACION

        private ICommand _cmdborrarregulacion;
        public ICommand cmdBorrarRegulacion {
            get {
                if (_cmdborrarregulacion == null) _cmdborrarregulacion = new RelayCommand(p => BorrarRegulacion(), p => PuedeBorrarRegulacion());
                return _cmdborrarregulacion;
            }
        }

        private bool PuedeBorrarRegulacion() {
            return RegulacionSeleccionada != null;
        }

        private void BorrarRegulacion() {
            _regulacionesborradas.Add(RegulacionSeleccionada);
            ConductorSeleccionado.ListaRegulaciones.Remove(RegulacionSeleccionada);
            HayCambios = true;
        }
        #endregion


        #region DESHACER BORRAR REGULACIONES
        private ICommand _cmddeshacerborrarregulaciones;
        public ICommand cmdDeshacerBorrarRegulaciones {
            get {
                if (_cmddeshacerborrarregulaciones == null) {
                    _cmddeshacerborrarregulaciones = new RelayCommand(p => DeshacerBorrarRegulaciones(), p => PuedeDeshacerBorrarRegulaciones());
                }
                return _cmddeshacerborrarregulaciones;
            }
        }

        private bool PuedeDeshacerBorrarRegulaciones() {
            return _regulacionesborradas.Count > 0;
        }

        private void DeshacerBorrarRegulaciones() {
            if (_regulacionesborradas == null) return;
            foreach (RegulacionConductor regulacion in _regulacionesborradas) {
                if (regulacion.Nuevo) {
                    ConductorSeleccionado.ListaRegulaciones.Add(regulacion);
                } else {
                    ConductorSeleccionado.ListaRegulaciones.Add(regulacion);
                    regulacion.Nuevo = false;
                }
                HayCambios = true;
            }
            _regulacionesborradas.Clear();
        }
        #endregion


        #region ACTUALIZAR LISTA
        // Comando
        private ICommand _cmdactualizarlista;
        public ICommand cmdActualizarLista {
            get {
                if (_cmdactualizarlista == null) _cmdactualizarlista = new RelayCommand(p => ActualizarLista(), p => PuedeActualizarLista());
                return _cmdactualizarlista;
            }
        }

        // Se puede ejecutar
        private bool PuedeActualizarLista() {
            if (App.Global.CentroActual == Centros.Desconocido) return false;
            return true;
        }

        // Ejecución del comando
        private void ActualizarLista() {

            GuardarConductores();
            Reiniciar();
        }
        #endregion


        #region AÑADIR REGULACION A TODOS
        // Comando
        private ICommand _cmdañadirregulacionatodos;
        public ICommand cmdAñadirRegulacionATodos {
            get {
                if (_cmdañadirregulacionatodos == null) _cmdañadirregulacionatodos = new RelayCommand(p => AñadirRegulacionATodos(), p => PuedeAñadirRegulacionATodos());
                return _cmdañadirregulacionatodos;
            }
        }

        // Se puede ejecutar
        private bool PuedeAñadirRegulacionATodos() {
            if (RegulacionSeleccionada == null) return false;
            return true;
        }

        // Ejecución del comando
        private void AñadirRegulacionATodos() {

            string mensaje = "Vas a insertar la siguiente regulacion a todos los conductores.\n\n";
            mensaje += $"Fecha: {RegulacionSeleccionada.Fecha:dd-MM-yyyy}\n";
            mensaje += $"Horas: {RegulacionSeleccionada.Horas.ToTexto(true)}\n";
            mensaje += $"DCs: {RegulacionSeleccionada.Descansos:0.0000}\n";
            mensaje += $"DNDs: {RegulacionSeleccionada.Dnds:0.0000}\n";
            mensaje += $"Motivo: {RegulacionSeleccionada.Motivo}\n\n";
            mensaje += "¿Desea continuar?";
            if (mensajes.VerMensaje(mensaje, "Añadir Regulacion a Todos", true) == true) {
                foreach (Conductor cond in ListaConductores) {
                    if (cond.Id != RegulacionSeleccionada.IdConductor) {
                        RegulacionConductor reg = new RegulacionConductor();
                        reg.Codigo = RegulacionSeleccionada.Codigo;
                        reg.Descansos = RegulacionSeleccionada.Descansos;
                        reg.Dnds = RegulacionSeleccionada.Dnds;
                        reg.Fecha = RegulacionSeleccionada.Fecha;
                        reg.Horas = RegulacionSeleccionada.Horas;
                        reg.Motivo = RegulacionSeleccionada.Motivo;
                        reg.IdConductor = cond.Id;
                        cond.ListaRegulaciones.Add(reg);
                    }
                }
            }
        }
        #endregion


        #region TEXTOMINUSCULAS

        // Comando
        private ICommand _cmdtextominusculas;
        public ICommand cmdTextoMinusculas {
            get {
                if (_cmdtextominusculas == null) _cmdtextominusculas = new RelayCommand(p => TextoMinusculas(), p => PuedeTextoMinusculas());
                return _cmdtextominusculas;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeTextoMinusculas() {
            return ListaConductores != null && ListaConductores.Count > 0;
        }

        // Ejecución del comando
        private void TextoMinusculas() {
            foreach (Conductor c in ListaConductores) {
                string nombre = c.Nombre.ToLower();
                string apellidos = c.Apellidos.ToLower();
                c.Nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombre);
                c.Apellidos = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(apellidos);
            }
        }
        #endregion


    } //Final de clase
}
