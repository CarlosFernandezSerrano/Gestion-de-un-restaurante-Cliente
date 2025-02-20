using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProtocolo
{
    public const string SEPARADOR = ":";

    public const int BIENVENIDA = 1;
    public const int NOMBRE = 2; //Server ->2:Dime tu nombre 
                                  //Cliente -> 2:Carlos

    public const int MENSAJE = 3;
    public const int PREG = 4;
    public const int RESP = 5;

    public const int FIN_CLIENTE = 100;
    public const int FIN_SERVER = 200;

    
}
