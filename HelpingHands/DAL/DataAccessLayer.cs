using HelpingHands.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml.Linq;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HelpingHands.DAL
{
    public class DataAccessLayer
    {
        SqlConnection _connection = null;
        SqlCommand _command = null;
        DataTable dt;
        SqlDataAdapter dbAdapter;
        public static IConfiguration Configuration { get; set; }
        
        public string GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            return Configuration.GetConnectionString("DefaultConnection");
        }

        //REGISTRATIONS AND LOGIN

        public bool RegisterUser(User user)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            { 
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddPatientUsers";

                _command.Parameters.AddWithValue("@UserName", user.UserName);
                _command.Parameters.AddWithValue("@Email", user.Email);
                _command.Parameters.AddWithValue("@Password", user.Password);
                _command.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);


                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id  > 0? true : false;
        }
		


		public string GetLogin(string UserName, string Password)
        {

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return null; 
            }
            using (_connection = new SqlConnection(GetConnectionString()))
            {
				_connection.Open();
				_command = _connection.CreateCommand();
				_command.CommandType = CommandType.StoredProcedure;
				_command.CommandText = "sp_GetLogin";


				_command.Parameters.AddWithValue("@UserName", UserName);
				_command.Parameters.AddWithValue("@Password", Password); 
                               
                using (SqlDataReader dr = _command.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        return dr["UserTypeDescription"].ToString();

                    }
                }
                _connection.Close();

			}
            return null;

        }

        //***********CITY***********
        public List<City> GetCities()
        {
            List<City> cityList = new List<City>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetCity";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    City city = new City();
                    city.CityId = Convert.ToInt32(dr["CityId"]);
                    city.CityName = dr["CityName"].ToString();
                    city.Abbreviation = dr["CityAbbreviation"].ToString();
                    

                    cityList.Add(city);
                }
                _connection.Close();
            }
            return cityList;
        }
       

        public bool InsertCity( City city)
        {
            int id = 0;
            using(_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddCity";

                _command.Parameters.AddWithValue("@CityName", city.CityName);
                _command.Parameters.AddWithValue("@Abbreviation", city.Abbreviation);

                _connection.Open();
                id =_command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool UpdateCity(City city)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpdateCity";

                _command.Parameters.AddWithValue("@CityId", city.CityId);
                _command.Parameters.AddWithValue("@CityName", city.CityName);
                _command.Parameters.AddWithValue("@Abbreviation", city.Abbreviation);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool DeleteCity(int id)
        {
            int affectedRow = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_DeleteCity";

                _command.Parameters.AddWithValue("@CityId", id);
                
                _connection.Open();
                affectedRow = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return affectedRow > 0 ? true : false;
        }


        public City GetCityById(int id)
        {
            City city = new City();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetCityById";

                _command.Parameters.AddWithValue("@CityId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    
                    city.CityId = Convert.ToInt32(dr["CityId"]);
                    city.CityName = dr["CityName"].ToString();
                    city.Abbreviation = dr["Abbreviation"].ToString();
                    

                    
                }
                _connection.Close();
            }
            return city;
        }

        //**********CHRONIC CONDITION******

        public List<ChronicCondition> GetCondition()
        {
            List<ChronicCondition> conditionList = new List<ChronicCondition>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetCity";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    ChronicCondition condition = new ChronicCondition();
                    condition.ConditionId = Convert.ToInt32(dr["ConditionId"]);
                    condition.ConditionName = dr["ConditionName"].ToString();
                    condition.ConditionDescr = dr["ConditionDescr"].ToString();


                    conditionList.Add(condition);
                }
                _connection.Close();
            }
            return conditionList;
        }
        public bool InsertCondition(ChronicCondition condition)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddCondition";

                _command.Parameters.AddWithValue("@CityName", condition.ConditionName);
                _command.Parameters.AddWithValue("@ConditionDescr", condition.ConditionDescr);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool UpdateCondition(ChronicCondition condition)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpdateCondition";

                _command.Parameters.AddWithValue("@ConditionId", condition.ConditionId);
                _command.Parameters.AddWithValue("@ConditionName", condition.ConditionName);
                _command.Parameters.AddWithValue("@ConditionDescr", condition.ConditionDescr);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool DeleteCondition(int id)
        {
            int affectedRow = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_DeleteCondition";

                _command.Parameters.AddWithValue("@ConditionId", id);

                _connection.Open();
                affectedRow = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return affectedRow > 0 ? true : false;
        }
        public ChronicCondition GetConditionById(int id)
        {
            ChronicCondition condition = new ChronicCondition();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetConditionById";

                _command.Parameters.AddWithValue("@ConditionId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    condition.ConditionId = Convert.ToInt32(dr["ConditionId"]);
                    condition.ConditionName = dr["ConditionName"].ToString();
                    condition.ConditionDescr = dr["ConditionDescr"].ToString();



                }
                _connection.Close();
            }
            return condition;
        }

        //***********SUBURB***********

        public List<Suburb> GetSuburb()
        {
            List<Suburb> suburbList = new List<Suburb>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetSuburb";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    Suburb suburb = new Suburb();
                    suburb.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    suburb.SuburbName = dr["SuburbId"].ToString();
                    suburb.PostalCode = dr["PostalCode"].ToString();
                    suburb.CityId = Convert.ToInt32(dr["CityId"]);


                    suburbList.Add(suburb);
                }
                _connection.Close();
            }
            return suburbList;
        }
        public bool InsertSuburb(Suburb suburb)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddSuburb";

                _command.Parameters.AddWithValue("@SuburbName", suburb.SuburbName);
                _command.Parameters.AddWithValue("@PostalCode", suburb.PostalCode);
                _command.Parameters.AddWithValue("@CityId", suburb.CityId);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;



            //int id = 0;

            //using (_connection = new SqlConnection(GetConnectionString()))
            //{
            //    _command = _connection.CreateCommand();
            //    _command.CommandType = CommandType.StoredProcedure;
            //    _command.CommandText = "sp_AddPatientUsers";

            //    _command.Parameters.AddWithValue("@UserName", user.UserName);
            //    _command.Parameters.AddWithValue("@Email", user.Email);
            //    _command.Parameters.AddWithValue("@Password", user.Password);
            //    _command.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);


            //    _connection.Open();
            //    id = _command.ExecuteNonQuery();
            //    _connection.Close();

            //}
            //return id > 0 ? true : false;







        }

        public bool UpdateSuburb(Suburb suburb)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpdateSuburb";

                _command.Parameters.AddWithValue("@SuburbId", suburb.SuburbId);
                _command.Parameters.AddWithValue("@SuburbName", suburb.SuburbName);
                _command.Parameters.AddWithValue("@PostalCode", suburb.PostalCode);
                _command.Parameters.AddWithValue("@CityId", suburb.CityId);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool DeleteSuburb(int id)
        {
            int affectedRow = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_DeleteSuburb";

                _command.Parameters.AddWithValue("@SuburbId", id);

                _connection.Open();
                affectedRow = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return affectedRow > 0 ? true : false;
        }
        public Suburb GetSuburbById(int id)
        {
            Suburb suburb = new Suburb();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetSuburbById";

                _command.Parameters.AddWithValue("@SuburbId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    suburb.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    suburb.SuburbName = dr["ConditionName"].ToString();
                    suburb.PostalCode = dr["ConditionDescr"].ToString();
                    suburb.CityId = Convert.ToInt32(dr["CityId"]);



                }
                _connection.Close();
            }
            return suburb;
        }

        //***********NURSE***********
        public List<Nurse> GetNurses()
        {
            List<Nurse> nurseList = new List<Nurse>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNurses";
                
                
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    Nurse nurse = new Nurse();
                    nurse.NurseId = Convert.ToInt32(dr["NurseId"]);
                    nurse.Name = dr["Name"].ToString();
                    nurse.Surname = dr["Surname"].ToString();
                    nurse.Gender = dr["Gender"].ToString();
                    nurse.IDNumber = dr["IDNumber"].ToString();
                    nurse.DoB = Convert.ToDateTime(dr["DoB"]).Date;

                    nurseList.Add(nurse);
                }
                _connection.Close();
            }
            return nurseList;
        }
    }
}
