/*    
    Obra original:
        Copyright (c) 2018 Packt
        Unity 2018 Artificial Intelligence Cookbook - Second Edition, by Jorge Palacios
        https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition
        MIT License

    Modificaciones:
        Copyright (C) 2020-2022 Federico Peinado
        http://www.federicopeinado.com

        Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
        Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
        Contacto: email@federicopeinado.com
*/
namespace es.ucm.fdi.iav.rts
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    // Puntos representativos o vértice (común a todos los esquemas de división, o a la mayoría de ellos)
    [System.Serializable]
    public class Vertex : MonoBehaviour, IComparable<Vertex>
    {
        /// <summary>
        /// Identificador del vértice 
        /// </summary>
        public int id;

        public float influence;
        public int controller;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Vertex other = obj as Vertex;
            if (other != null)
                return this.influence.CompareTo(other.influence);
            else
                throw new ArgumentException("Object is not a Vertex");
        }

        public int CompareTo(Vertex other)
        {
            return (int)(influence - other.influence);
        }

        public override bool Equals(object obj)
        {
            Vertex other = obj as Vertex;
            if (other != null)
                return id == other.id;
            else
                return false;
        }

        public bool Equals(Vertex other)
        {
            return other.id == id;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }
}
