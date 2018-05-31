using PRUEBA1;
using PRUEBA1.FSM;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvaPOS.FSM
{
    public class FSM
    {
        private const int NO_DEFINIDO = -1;
        private Nodo[,] _matrizTransiciones = null;
        private List<string> _estados;
        private List<string> _transiciones;
        private List<Accion>[] _accionesEntrada = null;
        private List<Accion>[] _accionesSalida = null;
        private string _estadoInicial = null;
        private int _indiceEstadoActual = NO_DEFINIDO;

        //Evento que contiene el estado actual
        public event TransitionedEvent OnTransitionedEvent;
        public delegate void TransitionedEvent(string estado);

        /// <summary>
        /// Nodo de la matriz de estados.
        /// </summary>
        private class Nodo
        {
            // Indice de estado en filas de matriz estados.
            public int IndiceEstado { get; set; } = NO_DEFINIDO;
            public List<Condicion> Condiciones { get; set; } = null;

            public bool NoDefinido { get { return (IndiceEstado == NO_DEFINIDO); } }
            public bool TieneCondiciones { get { return !Condiciones.IsNullOrEmptyList(); } }
        }

        private class Accion
        {
            public Action Lambda { get; }
            public string descripcion { get; } = "N/D";

            public Accion(Action lambda, string descripcion)
            {
                this.Lambda = lambda;
                this.descripcion = descripcion;
            }
        }

        private class Condicion
        {
            public Func<bool> Lambda { get; }
            public string descripcion { get; }

            public Condicion(Func<bool> lambda, string descripcion)
            {
                this.Lambda = lambda;
                this.descripcion = descripcion;
            }
        }

        public FSM(List<string> estados, List<string> transiciones, string estadoInicial)
        {
            _estados = estados;
            _transiciones = transiciones;
            _estadoInicial = estadoInicial;
            Inicializa();
        }

        public FSM(List<Enums.EstadosFSM> estados, List<Enums.TransicionesFSM> transiciones, Enums.EstadosFSM estadoInicial)
        {
            _estados = new List<string>();
            _transiciones = new List<string>();
            _estadoInicial = estadoInicial.GetEnumName();

            foreach (var item in transiciones)
            {
                _transiciones.Add(item.GetEnumName());
            }

            foreach (var itemEstado in estados)
            {
                _estados.Add(itemEstado.GetEnumName());
            }


            //
            Inicializa();
        }

        private void Inicializa()
        {
            Valida.ArgumentoNoNulo(_estados, "estados");
            Valida.ArgumentoNoNulo(_transiciones, "transiciones");
            Valida.ArgumentoNoNulo(_estadoInicial, "estadoInicial");
            int maxIndiceEstados = _estados.Count;
            int maxIndiceTransiciones = _transiciones.Count;
            _matrizTransiciones = new Nodo[maxIndiceEstados, maxIndiceTransiciones];
            // Se almacenan por estado.
            _accionesEntrada = new List<Accion>[maxIndiceEstados];
            // Se almacenan por estado.
            _accionesSalida = new List<Accion>[maxIndiceEstados];
            // Inicializar estados de transicion a no definido.
            for (int i = 0; i < maxIndiceEstados; i++)
            {
                for (int j = 0; j < maxIndiceTransiciones; j++)
                {
                    _matrizTransiciones[i, j] = new Nodo();
                }
            }
            // Estado inicial definido?.
            IndiceEstado(_estadoInicial);
            _indiceEstadoActual = _estados.IndexOf(_estadoInicial);
        }

        public string EstadoActual
        {
            get { return _estados[_indiceEstadoActual]; }
        }

        public EstadoFSM Definir(Enums.EstadosFSM estado)
        {
            return Definir(estado.GetEnumName());
        }

        public EstadoFSM Definir(string estado)
        {
            IndiceEstado(estado);
            var estadoFSM = new EstadoFSM(this, estado);
            return estadoFSM;
        }

        public void DefinirTransicion(string estado, string transicion, string estadoFinal)
        {
            _matrizTransiciones[IndiceEstado(estado), IndiceTransicion(transicion)].IndiceEstado = IndiceEstado(estadoFinal);
        }

        public void DefinirCondicion(string estado, string transicion, Func<bool> condicion, string descripcion)
        {
            Valida.TextoNoNuloVacio(descripcion, "Descripción");
            int indiceEstado = IndiceEstado(estado);
            int indiceTransicion = IndiceTransicion(transicion);

            if (_matrizTransiciones[indiceEstado, indiceTransicion].IndiceEstado == NO_DEFINIDO)
                throw new TransicionNoDefinidaException(
                    string.Format("Estado {0} no contiene transición {1}.",
                    estado,
                    transicion));
            if (_matrizTransiciones[indiceEstado, indiceTransicion].Condiciones == null)
                _matrizTransiciones[indiceEstado, indiceTransicion].Condiciones = new List<Condicion>();

            _matrizTransiciones[indiceEstado, indiceTransicion].Condiciones.Add(new Condicion(condicion, descripcion));
        }

        public void DefinirAccionEntrada(string estado, Action accion, string descripcion)
        {
            Valida.TextoNoNuloVacio(descripcion, "Descripción");
            int i = IndiceEstado(estado);
            // Estado inicial no permite acciones de entrada.
            if (estado.Equals(_estadoInicial))
                throw new ArgumentException("Estado inicial no permite definición de acciones de entrada.");
            if (_accionesEntrada[i] == null)
                _accionesEntrada[i] = new List<Accion>();
            _accionesEntrada[i].Add(new Accion(accion, descripcion));
        }

        public void DefinirAccionSalida(string estado, Action accion, string descripcion)
        {
            Valida.TextoNoNuloVacio(descripcion, "Descripción");
            int i = IndiceEstado(estado);
            if (_accionesSalida[i] == null)
                _accionesSalida[i] = new List<Accion>();
            _accionesSalida[i].Add(new Accion(accion, descripcion));
        }

        /// <summary>
        /// Transiciones definidas para el estado pasado como parámetro.
        /// </summary>
        /// <param name="estado"></param>
        /// <returns>Arreglo de string con transiciones.</returns>
        public List<string> TransicionesPorEstado(string estado)
        {
            int i = IndiceEstado(estado);
            var transiciones = new List<string>();
            for (int j = 0; j < _transiciones.Count; j++)
            {
                var nodo = _matrizTransiciones[i, j];
                if (!nodo.NoDefinido)
                    transiciones.Add(_transiciones[j]);
            }
            return transiciones;
        }


        public void LanzaTransicion(string transicion)
        {
            int indiceTransicion = IndiceTransicion(transicion);
            int indiceEstadoSiguiente = _matrizTransiciones[_indiceEstadoActual, indiceTransicion].IndiceEstado;
            // La transición esta definida en el estado actual?.
            if (indiceEstadoSiguiente == NO_DEFINIDO)
                throw new TransicionInvalidaException(
                    string.Format("Estado {0} no define transición {1}.",
                    _estados[_indiceEstadoActual],
                    transicion));
            // Si estado{transicion} tiene validación definida, la ejecuta. 
            // Si validación da negativa, excepción.
            List<Condicion> condiciones = _matrizTransiciones[_indiceEstadoActual, indiceTransicion].Condiciones;
            if (condiciones != null)
            {
                foreach (Condicion c in condiciones)
                {
                    bool ok = c.Lambda();
                    if (!ok) throw new CondicionFallidaException(
                        string.Format("Evaluación condición retorna false en estado {0} con transición {1}.",
                        _estados[_indiceEstadoActual],
                        transicion));
                }
            }
            // Si tiene acción de salida, y no hace transición al mismo estado, la ejecuta.
            // TODO Definir caso de manejo de excepciones en la acción de 
            //      hacer transición al mismo estado: ejecuta o no la salida?.
            List<Accion> acciones = null;
            if (_indiceEstadoActual != indiceEstadoSiguiente)
            {
                acciones = _accionesSalida[_indiceEstadoActual];
                if (acciones != null)
                {
                    foreach (Accion accion in acciones)
                    {
                        accion.Lambda();
                    }
                }
            }
            // La transición define un estado destino en matriz de transiciones.
            // Si tiene acción de entrada, la ejecuta (esté o no en el mismo 
            // estado antes de la transición).
            acciones = _accionesEntrada[indiceEstadoSiguiente];
            if (acciones != null)
            {
                foreach (Accion accion in acciones)
                {
                    accion.Lambda();
                }
            }
            // Llegado a este punto sin disparar excepciones, se procede a 
            // cambiar de estado.
            _indiceEstadoActual = indiceEstadoSiguiente;

            //Lanza evento de transición.
            this.OnTransitionedEvent?.Invoke(EstadoActual);
        }

        private int IndiceEstado(string estado)
        {
            Valida.TextoNoNuloVacio(estado, "Estado");
            int i = _estados.IndexOf(estado);
            if (i == -1)
                throw new EstadoNoDefinidoException(
                    string.Format("Estado {0} no definido.",
                    estado));
            return i;
        }

        private int IndiceTransicion(string transicion)
        {
            Valida.TextoNoNuloVacio(transicion, "Transición");
            int i = _transiciones.IndexOf(transicion);
            if (i == -1)
                throw new TransicionNoDefinidaException(
                    string.Format("Transición [{0}] no definida.",
                    transicion));
            return i;
        }

        public string GraficoDoT()
        {
            var sb = new StringBuilder();

            sb.Append("digraph {").AppendLine();
            for (int i = 0; i < _estados.Count; i++)
            {
                var acciones = _accionesEntrada[i];
                // Acciones de entrada.
                if (acciones.IsNullOrEmptyList() == false)
                {
                    var descrips = new List<string>();
                    foreach (Accion a in acciones)
                        descrips.Add(a.descripcion);

                    var txt = string.Join(", ", descrips);
                    sb.AppendFormat("\t\"[{0}]\" [shape=plaintext];", txt).AppendLine();
                    sb.AppendFormat("\t{0} -> \"[{1}]\" [label=\"Al entrar\" style=dotted, arrowhead=vee];",
                           _estados[i],
                           txt).AppendLine();

                }
                // Acciones de salida.
                acciones = _accionesSalida[i];
                if (acciones.IsNullOrEmptyList() == false)
                {
                    var descrips = new List<string>();
                    foreach (Accion a in acciones)
                        descrips.Add(a.descripcion);

                    var txt = string.Join(", ", descrips);
                    sb.AppendFormat("\t\"[{0}]\" [shape=plaintext];", txt).AppendLine();
                    sb.AppendFormat("\t{0} -> \"[{1}]\" [label=\"Al salir\" style=dotted, arrowhead=vee];",
                           _estados[i],
                           txt).AppendLine();

                }
                // Nodos y condiciones.
                for (int j = 0; j < _transiciones.Count; j++)
                {
                    var nodo = _matrizTransiciones[i, j];
                    if (nodo.NoDefinido)
                        continue;
                    if (nodo.TieneCondiciones)
                    {
                        var descrips = new List<string>();
                        foreach (Condicion c in nodo.Condiciones)
                            descrips.Add(c.descripcion);
                        sb.AppendFormat("\t{0} -> {1} [label=\"{2} Si: [{3}]\"];",
                            _estados[i],
                            _estados[nodo.IndiceEstado],
                            _transiciones[j],
                            string.Join(", ", descrips)).AppendLine();
                    }
                    else
                    {
                        sb.AppendFormat("\t{0} -> {1} [label=\"{2}\"];",
                            _estados[i],
                            _estados[nodo.IndiceEstado],
                            _transiciones[j]).AppendLine();

                    }
                }
            }
            sb.Append("}");

            return sb.ToString();
        }
    }
}
