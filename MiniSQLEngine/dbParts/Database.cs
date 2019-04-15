﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using MiniSQLEngine;
using System.ComponentModel;


namespace MiniSQLEngine
{
    public class Database : IDisposable
    {
        private Boolean disposed = false;
        public String Name { get; }
        private List<Table> Tables = new List<Table>();
        // Track whether Dispose has been called.        

        public Database(string dbName)
        {
            Name = dbName;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing == true)
                {
                    //someone want the deterministic release of all resources
                    //Let us release all the managed resources
                    ReleaseManagedResources();
                }
                else
                {
                    // Do nothing, no one asked a dispose, the object went out of
                    // scope and finalized is called so lets next round of GC 
                    // release these resources

                }
                // Release the unmanaged resource in any case as they will not be 
                // released by GC
                disposed = true;
                ReleaseUnmanagedResources();

            }
        }

        private void ReleaseUnmanagedResources()
        {
            Console.WriteLine("Releasing Managed Resources");
            if (this != null)
            {
                this.Dispose();
            }
        }
        private void ReleaseManagedResources()
        {
            FileSystemAbstract.saveData(Name,Tables);
            Console.WriteLine("Releasing Unmanaged Resources");
        }


        /// <summary>
        /// Get a table by its name
        /// </summary>
        /// <param name="name">The name of the table</param>
        /// <returns>Returns the table or null if not found</returns>
        public Table GetTableByName(String name)
        {
            foreach (Table table in Tables)
            {
                if (table.Name==name)
                {
                    return table;
                }
            }
            return null ;
        }

        /// <summary>
        /// Creates a table
        /// </summary>
        /// <param name="name">Name of the new table</param>
        /// <param name="columns">Columns to be added to the table</param>
        /// <returns>returns an error/success message</returns>
        public string CreateTable(string name, List<Column> columns)
        {
            if (GetTableByName(name) != null)
                return Messages.TableErrorAlreadyExists;
            else
            {
                try
                {
                    Table table = new Table(name, columns);
                    Tables.Add(table);
                    return Messages.CreateTableSuccess;
                }
                catch
                {
                    return Messages.WrongSyntax;
                }
            }
        }

        public string Update(String columns, String tableName, String left, String op, String right)
        {
            Table table = GetTableByName(tableName);
            table.ColumnByName(columns);

            //paso los mismos parametros que me han enviado 

            table.Update(columns,tableName, left, op, right);
            return "hay que cambiarlo";
        }
        public Table SelectAll(string tableName)
        {
            return GetTableByName(tableName);
        }
        public Table SelectColumns(string tableName, List<string> columnNames)
        {
            Table sourceTable = GetTableByName(tableName);
            List<Column> selectedColumns = new List<Column>();
            //Else only selected ones
            foreach (string columnName in columnNames)
            {
                if (columnName == "*")
                    return null; // we don't allow "SELECT *,Name ...", only "SELECT * ..."

                Column column = sourceTable.ColumnByName(columnName);
                if (column != null)
                    selectedColumns.Add(column);
                else return null;
            }
            Table result = new Table("Result",selectedColumns);
            return result;
        }

        public string Insert(string tableName, string [] values)
        {
            Table table = GetTableByName(tableName);
            if (table == null)
            {
                return Messages.TableDoesNotExist;
            }
            int cont = 0;

            foreach (Column column in table.Columns)
            {
                column.AddValue(values[cont]);
                cont++;
            }
            return Messages.InsertSuccess;
        }

        public string Insert(string tableName, List<string> columnNames, string[] values)
        {
            Table table = GetTableByName(tableName);
            if (table == null)
            {
                return Messages.TableDoesNotExist;
            }
            

            foreach (Column column in table.Columns)
            {
                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (column.Name == columnNames[i])
                    {
                        column.AddValue(values[i]);
                    }
                }
               

            }
            return Messages.InsertSuccess;
        }


        public Table DeleteRows(String tableName, String left, String op, string right)
        {
            Table sourceTable = GetTableByName(tableName);
            sourceTable.DeleteRows(left, op, right);
            return sourceTable;

        }

        public Table DeleteTable(string name)
        {
            for (int i = 0; i < Tables.Count; i++)
            {
                if (Tables[i].Name.Equals(name))
                {
                    Tables.RemoveAt(i);
                }
            }
            return null;
        }


        public String RunQuery(string line)
        {
            Query theQuery = Parser.Parse(line);
            FileSystemAbstract.saveData(Name, Tables);
            return theQuery.Run(this);
            
        }
    }
}