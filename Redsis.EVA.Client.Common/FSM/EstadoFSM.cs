using System;
using System.Collections.Generic;
using System.Text;

namespace EvaPOS.FSM
{
    public class EstadoFSM
    {
        private FSM _fsm = null;
        private string _estado = null;

        public EstadoFSM(FSM fsm, string estado)
        {
            _fsm = fsm;
            _estado = estado;
        }

        public EstadoFSM Transicion(Enums.TransicionesFSM transicion, Func<Enums.EstadosFSM> estadoFinal)
        {
            string estadoStr = estadoFinal().GetEnumName();
            return Transicion(transicion.GetEnumName(), estadoStr);
        }

        public EstadoFSM Transicion(Enums.TransicionesFSM transicion, Enums.EstadosFSM estadoFinal)
        {
            return Transicion(transicion.GetEnumName(), estadoFinal.GetEnumName()); 
        }

        public EstadoFSM Transicion(string transicion, string estadoFinal)
        {
            _fsm.DefinirTransicion(_estado, transicion, estadoFinal);
            return this;
        }

        public EstadoFSM TransicionCondicionada(string transicion, string estadoFinal, Func<bool> condicion, string descripcion)
        {
            _fsm.DefinirTransicion(_estado, transicion, estadoFinal);
            _fsm.DefinirCondicion(_estado, transicion, condicion, descripcion);
            return this;
        }

        public EstadoFSM AlEntrar(Action accion, string descripcion)
        {
            _fsm.DefinirAccionEntrada(_estado, accion, descripcion);
            return this;
        }

        public EstadoFSM AlEntrar(Action accion, Enums.EstadosFSM estado, string descripcion)
        {
            _fsm.DefinirAccionEntrada(estado.GetEnumName(), accion, descripcion);
            return this;
        }

        public EstadoFSM AlSalir(Action accion, string descripcion)
        {
            _fsm.DefinirAccionSalida(_estado, accion, descripcion);
            return this;
        }
    }
}
