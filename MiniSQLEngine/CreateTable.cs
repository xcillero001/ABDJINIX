﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MiniSQLEngine;

public class CreateTable : Query
{
    
    String pTabla;
    List<String> columnNames = new List<String>();
    List<String> pTipoDato = new List<String>();
  
    public CreateTable(String tabla, String tipoDato)
    {

        pTabla = tabla;
        String[] splitParameters = tipoDato.Split(',');
        String[] splitNames;
        String[] splitTypes;

    }
    public override String Run(Database bd)
    {

        //TODO: Create columns one by one in a list and call bd.CreateDatabase
        if(bd.GetTableByName(pTabla) != null)
        {
            return Messages.TableErrorAlreadyExists;
        }

        

        


        return null;
    }
    public string getTabla()
    {
        return pTabla;
    }

}