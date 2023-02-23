

using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
	
	
	namespace EjemploConexiónBDAccess
{
		
		// Clase que gestiona la comunicación con la base de datos
		public class BDMySql
		{
			
			private MySqlConnection conexion;
			private static BDMySql instance=null;
			public const string SERVER = "localhost";
			public const int PUERTO = 3306;
			public const string USUARIO = "root";
			public const string CLAVE = "root";
			public const string NOMBRE_BD = "PONER AQUÍ NOMBRE BASE DE DATOS";
		

		public static BDMySql getInstance()
		{
			if (instance == null)
			{
				instance = new BDMySql(SERVER,NOMBRE_BD,USUARIO,CLAVE,PUERTO);
			}

			return instance;
		}

		// En el constructor se establece la cadena de conexión
		private BDMySql(string servidor, string NombreBD, string usuario, string passwd, int  puerto = 3306)
		{
			try
			{
				conexion = new MySqlConnection();
				conexion.ConnectionString = string.Format("server={0};port={1};database={2};Uid={3};Pwd={4};", servidor, puerto, NombreBD, usuario, passwd);
			}
			catch (MySqlException ex)
            
			{
				MessageBox.Show("Error al conectar al servidor MySQL " + "\r\n" + "\r\n" + ex.Message,"Atención", MessageBoxButton.OK,MessageBoxImage.Warning);
			}
		}
		
		// Se intenta abrir la conexión con la base de datos
		public bool AbrirConexion()
		{
			try // No se puede abrir una conexión ya abierta
			{
				if (conexion.State != ConnectionState.Open)
				{
					conexion.Open();
				}
				return true;
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Error intentando abrir la Base de Datos" + "\r\n" + "\r\n" + ex.Message, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
				return false;
			}
		}
		
		
		// Chuquea si la conexión está abierta
		public bool ConexionAbierta()
		{
			if (conexion != null)
			{
				if (conexion.State != ConnectionState.Broken & conexion.State != ConnectionState.Closed)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		
		//Cierra la conexión.
		public bool CerrarConexion()
		{
			try // No se puede cerrar un conexión ya cerrada
			{
				if (conexion.State != ConnectionState.Closed)
				{
					conexion.Close();
					return true;
				}
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Error intentando cerrar la Base de Datos" + "\r\n" + "\r\n" + ex.Message, "Error", MessageBoxButton.OK , MessageBoxImage.Error);
			}
            return false;
		}
		
		
		//Lanza una sentencia SELECT y devuelve las filas y columnas en un objeto DataTable
		//El fillschema te traslada la info de la tabla a memoria, es decir, longuitudes máximas, etc que estableciste al crear esa tabla, 
		//si le pongo false solo traslada los datos y no la info sobre la tabla
		public DataTable LanzaSelect(string sentenciaSELECT, bool fillschema)
		{
			MySqlDataAdapter DA; // Permite lanzar la sentencia SELECT
			DataTable tabla = new DataTable();
			try
			{
				DA = new MySqlDataAdapter(sentenciaSELECT, conexion);
				// Solo se puede agregar el esquema si la sentencia SELECT es sobre una tabla
				if (fillschema)
				{
					DA.FillSchema(tabla, SchemaType.Mapped); //Introducimos información (como longitudes máximas de campos de texto) del esquema de la tabla
				}
				DA.Fill(tabla); // Rellemanos el data set con el resultado del SELECT
				return tabla;
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Error al ejecutar la consulta" + "\r\n" + "\r\n" + ex.Message,"Error", MessageBoxButton.OK , MessageBoxImage.Error);
				return null;
			}

			/*  Como recorrer las filas de un DataTable:
			    foreach (DataRow fila  in datosEmpleados.Rows)
                {
			       // podemos extraer los datos por número de columna o nombre, ej: fila[1] ó fila["nombre_empleado"]  
			    }
			*/
		}
		
		//Rellena de nuevo un DataTable
		public void RefrescaTabla(string SQL_SELECT, DataTable tabla)
		{
			try
			{
				MySqlDataAdapter adaptador = new MySqlDataAdapter(SQL_SELECT, conexion);
				tabla.Rows.Clear();
				adaptador.Fill(tabla);
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Error al actualizar los datos de la tabla " + tabla.TableName + "\r\n" + "\r\n" + ex.Message, "Error", MessageBoxButton.OK ,MessageBoxImage.Error);
			}
		}
		
		
		// Lanzas los oportunos INSERT,DELETE,UPDATE para actualizar la tabla de la BD según los datos contenidos en el Datatable
		public void ActualizaDatosTabla(string SQL_SELECT, DataTable tabla)
		{
			MySqlDataAdapter adaptador = new MySqlDataAdapter(SQL_SELECT, conexion);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(adaptador);
			cb.RefreshSchema();
			adaptador.InsertCommand = cb.GetInsertCommand();
			adaptador.UpdateCommand = cb.GetUpdateCommand();
			adaptador.DeleteCommand = cb.GetDeleteCommand();
			adaptador.Update(tabla);
			adaptador.Fill(tabla); // Los triggers de la base de datos podrían modificar los datos o aparecer/desaparacer nuevas filas.
			//si le mando una nueva fila y la base de datos ha modificado algo, tipo el autoincremental, te lo devuelve para que tengas los nuevos datos en memoria
		}
		
		// Permite lanzar consulas INSERT,UPDATE,DELETE. Devuelve el número de filas afectadas.
		public int EjecutaConsultaAccion(string sentenciaAccion)// esto es para trabajar de manera de conexion directa, no la usariamos
		{
			int filasAfectadas = 0;
			MySqlCommand comandoSQL = new MySqlCommand();
			try
			{
				comandoSQL.Connection = conexion;
				comandoSQL.CommandText = sentenciaAccion;
				filasAfectadas = comandoSQL.ExecuteNonQuery();
				return filasAfectadas;
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Error al ejecutar la consulta" + "\r\n" + "\r\n" + ex.Message,"Error", MessageBoxButton.OK , MessageBoxImage.Error);
				return -1;
			}
		}
		
		//Permite lanzar una sentencia SELECT que devuelve solo un dato (una fila y una columna)
		public object AccionEscalar(string sentencia)
		{
			var comandoSQL = new MySqlCommand();
			try
			{
				comandoSQL.Connection = conexion;
				comandoSQL.CommandText = sentencia;
				return comandoSQL.ExecuteScalar();
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Error al ejecutar la consulta" + "\r\n" + "\r\n" + ex.Message, "Error", MessageBoxButton.OK , MessageBoxImage.Error);
				return null;
			}
		}
		
		
	}
	
	
}
