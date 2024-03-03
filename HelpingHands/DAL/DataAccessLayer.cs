using HelpingHands.Models;
using HelpingHands.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

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
        private readonly IHttpContextAccessor _contextAccessor;
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

		public User GetLogin(string UserName, string Password)
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
					if (dr.Read())
					{
						User user = new User
						{
							UserId = (int)dr["UserId"], 
							UserTypeDescription = dr["UserTypeDescription"].ToString(),
                            UserName = dr["UserName"].ToString()
						};
						return user;
					}
				}
			}
			return null;
		}

        public List<ChangePassword> GetUsersForPassword()
        {
            List<ChangePassword> allUsers = new List<ChangePassword>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetUsersForPassword";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    ChangePassword user = new ChangePassword();
                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.Password = dr["Password"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();

                    allUsers.Add(user);
                }
                _connection.Close();
            }
            return allUsers;
        }
        public bool ChangePassword(ChangePassword user)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_ChangePassword";

                _command.Parameters.AddWithValue("@UserId", user.UserId);
                _command.Parameters.AddWithValue("@Password", user.Password);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }

        public int GetUserId(string UserName)
        {
            int id = 0;

            try
            {
                using (_connection = new SqlConnection(GetConnectionString()))
                using (_command = _connection.CreateCommand())
                {
                    _connection.Open();
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = "sp_GetUserId";

                    _command.Parameters.AddWithValue("@UserName", UserName);
                    

                    using (SqlDataReader dr = _command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            id = Convert.ToInt32(dr["UserId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return id;
        }

        public int GetPatientId(int userId)
        {
            int id = 0;

            try
            {
                using (_connection = new SqlConnection(GetConnectionString()))
                using (_command = _connection.CreateCommand())
                {
                    _connection.Open();
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = "sp_GetPatientId";

                    _command.Parameters.AddWithValue("@UserId", userId);


                    using (SqlDataReader dr = _command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return Convert.ToInt32(dr["PatientId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return id;
        }

        public int GetNurseId(int userId)
        {
            int id = 0;

            try
            {
                using (_connection = new SqlConnection(GetConnectionString()))
                using (_command = _connection.CreateCommand())
                {
                    _connection.Open();
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = "sp_GetNurseId";

                    _command.Parameters.AddWithValue("@UserId", userId);


                    using (SqlDataReader dr = _command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return Convert.ToInt32(dr["NurseCode"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return id;
        }

        public UserVM GetEmployeeById(int id)
        {
            UserVM user = new UserVM();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetEmployeeById";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();


                }
                _connection.Close();
            }
            return user;
        }

        public ChangePassword GetUserById(int id)
        {
            ChangePassword user = new ChangePassword();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetUserById";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.Password = dr["Password"].ToString();
                    user.Status = dr["Status"].ToString();
                }
                _connection.Close();
            }
            return user;
        }
        public bool AddPatientProfile(Patient patient)
        {
            int id = 0;            
            
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddPatientProfile";

                _command.Parameters.AddWithValue("@UserId", patient.UserId);
                _command.Parameters.AddWithValue("@Surname", patient.Surname);
                _command.Parameters.AddWithValue("@FirstName", patient.Name);
                _command.Parameters.AddWithValue("@Gender", patient.Gender);
                _command.Parameters.AddWithValue("@IdNumber", patient.IDNumber);
                _command.Parameters.AddWithValue("@DoB", patient.DoB.ToString());
                _command.Parameters.AddWithValue("@EmergencyContactP", patient.ContactPerson); 
                _command.Parameters.AddWithValue("@EmargencyContactNum", patient.ContactPersonNumber);
                _command.Parameters.AddWithValue("@AdditionalInformation", patient.AdditionalInform);
                _command.Parameters.AddWithValue("@Picture", patient.Picture);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }

        public bool UpdatePatientProfile(Patient patient)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_UpdatePatientProfile]";



                _command.Parameters.AddWithValue("@UserId", patient.UserId);
                _command.Parameters.AddWithValue("@Surname", patient.Surname);
                _command.Parameters.AddWithValue("@FirstName", patient.Name);
                _command.Parameters.AddWithValue("@Gender", patient.Gender);
                _command.Parameters.AddWithValue("@IdNumber", patient.IDNumber);
                _command.Parameters.AddWithValue("@DoB", patient.DoB.ToString());
                _command.Parameters.AddWithValue("@EmergencyContactP", patient.ContactPerson);
                _command.Parameters.AddWithValue("@EmargencyContactNum", patient.ContactPersonNumber);
                _command.Parameters.AddWithValue("@AdditionalInformation", patient.AdditionalInform);
                _command.Parameters.AddWithValue("@Picture", patient.Picture);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }
        public Patient GetPatientProfile(int? userId)
        {

            Patient patient = new Patient();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetPatientById";

                _command.Parameters.AddWithValue("@UserId", userId);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    patient.UserId = Convert.ToInt32(dr["UserId"]);
                    patient.Surname = dr["Surname"].ToString();
                    patient.Name = dr["Firstname"].ToString();
                    patient.Gender = dr["Gender"].ToString();
                    patient.IDNumber = dr["IDNumber"].ToString();
                    patient.DoB = Convert.ToDateTime(dr["DateOfBirth"]).Date;
                    patient.ContactPerson = dr["EmergencyContactPerson"].ToString();
                    patient.ContactPersonNumber = dr["EmergencyContactNumber"].ToString();
                    patient.AdditionalInform = dr["AdditionalInformation"].ToString();
                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        patient.Picture = (byte[])dr["Picture"];
                    }


                }
                _connection.Close();
            }
            return patient;
                       
            
        }

        public UserDetails GetPatientDetailsById(int id)
        {
            UserDetails patient = new UserDetails();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetPatientDetails]";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    patient.UserId = Convert.ToInt32(dr["PatientId"]);
                    patient.Surname = dr["Surname"]?.ToString();
                    patient.Name = dr["FirstName"]?.ToString();
                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        patient.Picture = (byte[])dr["Picture"];
                    }

                }
                _connection.Close();
            }
            return patient;
        }

        public bool AddNurseProfile(Nurse nurse)
        {
            int id = 0;


            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddNurseProfile";

                _command.Parameters.AddWithValue("@UserId", nurse.UserId);
                _command.Parameters.AddWithValue("@Surname", nurse.Surname);
                _command.Parameters.AddWithValue("@FirstName", nurse.Name);
                _command.Parameters.AddWithValue("@Gender", nurse.Gender);
                _command.Parameters.AddWithValue("@IdNumber", nurse.IDNumber);
                _command.Parameters.AddWithValue("@Picture", nurse.Picture);


                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }

        public bool UpdateNurseProfile(Nurse nurse)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_UpdateNurseProfile]";



                _command.Parameters.AddWithValue("@UserId", nurse.UserId);
                _command.Parameters.AddWithValue("@Surname", nurse.Surname);
                _command.Parameters.AddWithValue("@FirstName", nurse.Name);
                _command.Parameters.AddWithValue("@Gender", nurse.Gender);
                _command.Parameters.AddWithValue("@IdNumber", nurse.IDNumber);
                _command.Parameters.AddWithValue("@Picture", nurse.Picture);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }
        public Nurse GetNurseProfile(int id)
        {
            Nurse nurseDetails = new Nurse();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNurseProfile";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    nurseDetails.UserId = Convert.ToInt32(dr["UserId"]);
                    nurseDetails.Surname = dr["Surname"].ToString();
                    nurseDetails.Name = dr["FirstName"].ToString();
                    nurseDetails.Gender = dr["Gender"].ToString();
                    nurseDetails.IDNumber = dr["IDNumber"].ToString();
                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        nurseDetails.Picture = (byte[])dr["Picture"];
                    }

                }
                _connection.Close();
            }
            return nurseDetails;
        }
        public Nurse GetNursePicture(int id)
        {
            Nurse nurseDetails = new Nurse();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetPicture";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {                   
                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        nurseDetails.Picture = (byte[])dr["Picture"];
                    }

                }
                _connection.Close();
            }
            return nurseDetails;
        }

        public Nurse GetNurseDetailsById(int id)
        {
            Nurse nurse = new Nurse();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetNurseDetails]";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    nurse.NurseId = Convert.ToInt32(dr["NurseCode"]);
                    nurse.Surname = dr["Surname"].ToString();
                    nurse.Name = dr["FirstName"].ToString();
                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        nurse.Picture = (byte[])dr["Picture"];
                    }


                }
                _connection.Close();
            }
            return nurse;
        }


        public bool AddEmployee(User user)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_InsertEmployee";

                _command.Parameters.AddWithValue("@UserName", user.UserName);
                _command.Parameters.AddWithValue("@Email", user.Email);
                _command.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);
                _command.Parameters.AddWithValue("@Password", user.Password);
                _command.Parameters.AddWithValue("@UserTypeId", user.UserTypeId);
                _command.Parameters.AddWithValue("@Status", user.Status);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }
		public bool UpdateEmployee(UserVM user)
        { 
			int id = 0;

			using (_connection = new SqlConnection(GetConnectionString()))
			{
				_command = _connection.CreateCommand();
				_command.CommandType = CommandType.StoredProcedure;
				_command.CommandText = "sp_UpdateEmployee";


                _command.Parameters.AddWithValue("@UserId", user.UserId);
                _command.Parameters.AddWithValue("@UserName", user.UserName);
				_command.Parameters.AddWithValue("@Email", user.Email);
                _command.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);
				_command.Parameters.AddWithValue("@UserTypeId", user.UserTypeId);
                

                _connection.Open();
				id = _command.ExecuteNonQuery();
				_connection.Close();

			}
			return id > 0 ? true : false;
		}

        public bool DeleteUser(UserVM user)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_DeleteUser";

                _command.Parameters.AddWithValue("@UserId", user.UserId);
                _command.Parameters.AddWithValue("@Status", user.Status);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }

        public bool ActivateUser(UserVM user)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_ActivateUser]";

                _command.Parameters.AddWithValue("@UserId", user.UserId);
                _command.Parameters.AddWithValue("@Status", user.Status);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }

        public List<UserType> GetUserType()
        {
            List<UserType> userTypes = new List<UserType>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetUserType";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    UserType type = new UserType();
                    type.UserTypeId = Convert.ToInt32(dr["UserTypeId"]);
                    type.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    type.Abbreviation = dr["Abbreviation"].ToString();


                    userTypes.Add(type);
                }
                _connection.Close();
            }
            return userTypes;
        }

        public List<UserVM> GetAllEmployees()
        {
            List<UserVM> allUsers = new List<UserVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetAllEmployees";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    UserVM user = new UserVM();
                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();

                    allUsers.Add(user);
                }
                _connection.Close();
            }
            return allUsers;
        }
        public List<UserVM> GetAllUsers()
        {
            List<UserVM> allUsers = new List<UserVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetAllUsers";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    UserVM user = new UserVM();
                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();

                    allUsers.Add(user);
                }
                _connection.Close();
            }
            return allUsers;
        }

        public List<UserVM> GetDeletedUsers()
        {
            List<UserVM> allUsers = new List<UserVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetDeletedUser";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    UserVM user = new UserVM();
                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();

                    allUsers.Add(user);
                }
                _connection.Close();
            }
            return allUsers;
        }

        public List<UserVM> SearchUsers(string search)
        {
            List<UserVM> allUsers = new List<UserVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_SearchUsers";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                _command.Parameters.AddWithValue("@search", search);
                

                while (dr.Read())
                {
                    UserVM user = new UserVM();
                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();

                    allUsers.Add(user);
                }
                _connection.Close();
            }
            return allUsers;
        }
              

        public Nurse GetNurseById(int id)
        {
            Nurse nurse = new Nurse();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNurseById";

                _command.Parameters.AddWithValue("@NurseCode", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    nurse.NurseId = Convert.ToInt32(dr["NurseCode"]);
                    nurse.Surname = dr["Surname"].ToString();
                    nurse.Name = dr["Firstname"].ToString();
                    nurse.Gender = dr["Gender"].ToString();
                    nurse.IDNumber = dr["IdNumber"].ToString();



                }
                _connection.Close();
            }
            return nurse;
        }
               
        

        public bool UpdatePassword(User user)
        {
            int id = 0;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_UpdatePassword]";



                _command.Parameters.AddWithValue("@UserId", user.UserId);
                _command.Parameters.AddWithValue("@Password", user.Password);
               

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();

            }
            return id > 0 ? true : false;
        }

        public User GetPatientById(int id)
        {
            User user = new User();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetEmployeeById";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    user.UserId = Convert.ToInt32(dr["UserId"]);
                    user.UserName = dr["UserName"].ToString();
                    user.Email = dr["Email"].ToString();
                    user.ContactNumber = dr["ContactNumber"].ToString();
                    user.UserTypeDescription = dr["UserTypeDescription"].ToString();
                    user.Status = dr["Status"].ToString();


                }
                _connection.Close();
            }
            return user;
        }

        //***********BUSINESS INFORMATION*************

        public List<HelpingHandsBusiness> GetBusinessInformation()
        {
            List<HelpingHandsBusiness> businessInfo = new List<HelpingHandsBusiness>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetBusinessInfo";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();
                

                while (dr.Read())
                {
                    HelpingHandsBusiness help = new HelpingHandsBusiness();
                    help.businessId = Convert.ToInt32(dr["BusinessId"]);
                    help.orgName = dr["OrganisationName"].ToString();
                    help.npoNumber = dr["NPONumber"].ToString();
                    help.Email = dr["Email"].ToString();
                    help.address = dr["Address"].ToString();
                    help.contactNumber = dr["ContactNumber"].ToString();
                    help.operatingHours = dr["OperatingHours"].ToString();

                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        help.picture = (byte[])dr["Picture"];
                    }


                    businessInfo.Add(help);
                }
                _connection.Close();
            }
            return businessInfo;
        }
        public bool InsertBusinessInformation(HelpingHandsBusiness business)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_InsertBusinessInformation";

                _command.Parameters.AddWithValue("@OrganisationName", business.orgName);
                _command.Parameters.AddWithValue("@NPONumber", business.npoNumber);
                _command.Parameters.AddWithValue("@Address", business.address);
                _command.Parameters.AddWithValue("@Email", business.Email);
                _command.Parameters.AddWithValue("@ContactNumber", business.contactNumber);
                _command.Parameters.AddWithValue("@OperatingHours", business.operatingHours);
                _command.Parameters.AddWithValue("@Picture", business.picture);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool UpdateBusinessInformation(HelpingHandsBusiness business)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpdateBusinessInformation";

                _command.Parameters.AddWithValue("@BusinessId", business.businessId);
                _command.Parameters.AddWithValue("@OrganisationName", business.orgName);
                _command.Parameters.AddWithValue("@NPONumber", business.npoNumber);
                _command.Parameters.AddWithValue("@Email", business.Email);
                _command.Parameters.AddWithValue("@Address", business.address);
                _command.Parameters.AddWithValue("@ContactNumber", business.contactNumber);
                _command.Parameters.AddWithValue("@OperatingHours", business.operatingHours);
                _command.Parameters.AddWithValue("@Picture", business.picture);


                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }
        public HelpingHandsBusiness GetBusinessById(int id)
        {
            HelpingHandsBusiness help = new HelpingHandsBusiness();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetBusinessById";

                _command.Parameters.AddWithValue("@BusinessId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    help.businessId = Convert.ToInt32(dr["BusinessId"]);
                    help.orgName = dr["OrganisationName"].ToString();
                    help.npoNumber = dr["NPONumber"].ToString();
                    help.Email = dr["Email"].ToString();
                    help.address = dr["Address"].ToString();
                    help.contactNumber = dr["ContactNumber"].ToString();
                    help.operatingHours = dr["OperatingHours"].ToString();

                    if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
                    {
                        help.picture = (byte[])dr["Picture"];
                    }




                }
                _connection.Close();
            }
            return help;
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
                _command.Parameters.AddWithValue("@CityAbbreviation", city.Abbreviation);

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
                _command.Parameters.AddWithValue("@CityAbbreviation", city.Abbreviation);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }


            if(id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

            
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
                    city.Abbreviation = dr["CityAbbreviation"].ToString();
                    

                    
                }
                _connection.Close();
            }
            return city;
        }


        //***********SUBURB***********

        public List<Suburb> GetSuburb()
        {
            List<Suburb> suburbList = new List<Suburb>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetSuburbs";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    Suburb suburb = new Suburb();
                    suburb.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    suburb.SuburbName = dr["SuburbName"].ToString();
                    suburb.PostalCode = dr["PostalCode"].ToString();
                    suburb.CityName = dr["CityName"].ToString();


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
                    suburb.SuburbName = dr["SuburbName"].ToString();
                    suburb.PostalCode = dr["PostalCode"].ToString();
                    suburb.CityName = dr["CityName"].ToString();



                }
                _connection.Close();
            }
            return suburb;
        }

        public List<Suburb> GetSuburbByCityId(int id)
        {
            List<Suburb> suburb = new List<Suburb>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetSuburbByCityId";

                _command.Parameters.AddWithValue("@CityId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    Suburb suburbs = new Suburb();

                    suburbs.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    suburbs.SuburbName = dr["SuburbName"].ToString();
                    
                    suburb.Add(suburbs);


                }
                _connection.Close();
            }
            return suburb;
        }

        public bool InsertPreferredSuburb(PrefferedSuburb suburb)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_InsertPreferredSuburb";

                _command.Parameters.AddWithValue("@SuburbId", suburb.SuburbId);
                _command.Parameters.AddWithValue("@NurseId", suburb.NurseId);
                

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;

        }

        public List<PrefferedSuburb> GetSuburbByNurseId(int nurseId)
        {
            List<PrefferedSuburb> suburbList = new List<PrefferedSuburb>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetSuburbByNurseId";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    PrefferedSuburb suburb = new PrefferedSuburb();
                    suburb.PrefSuburbId = Convert.ToInt32(dr["PreferredSuburbId"]);
                    suburb.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    suburb.SuburbName = dr["SuburbName"].ToString();


                    suburbList.Add(suburb);
                }
                _connection.Close();
            }
            return suburbList;
        }

        public bool DeletePreferredSuburb(int id, int nurseId)
        {
            int affectedRow = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_DeletePreferredSuburb";

                _command.Parameters.AddWithValue("@PreferredSuburbId", id);
                _command.Parameters.AddWithValue("@NurseId", nurseId);

                _connection.Open();
                affectedRow = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return affectedRow > 0 ? true : false;
        }

        public PrefferedSuburb GetPreferredSuburbById(int id)
        {
            PrefferedSuburb prefferedSuburb = new PrefferedSuburb();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetPreferredSuburbById";

                _command.Parameters.AddWithValue("@PreferredSuburbId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    prefferedSuburb.PrefSuburbId = Convert.ToInt32(dr["PreferredSuburbId"]);
                    prefferedSuburb.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    prefferedSuburb.SuburbName = dr["SuburbName"].ToString();



                }
                _connection.Close();
            }
            return prefferedSuburb;
        }

        public List<PreferredSuburb> GetNurseSuburbs(int nurseId)
        {
            List<PreferredSuburb> suburbList = new List<PreferredSuburb>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNurseSuburb";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    PreferredSuburb suburbs = new PreferredSuburb();
                    suburbs.PrefSuburbId = Convert.ToInt32(dr["PreferredSuburbId"]);
                    suburbs.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    suburbs.SuburbName = dr["SuburbName"].ToString();
                   

                    suburbList.Add(suburbs);
                }
                _connection.Close();
            }
            return suburbList;
        }



        //**********CHRONIC CONDITION******

        public List<ChronicCondition> GetCondition()
        {
            List<ChronicCondition> conditionList = new List<ChronicCondition>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetCondition";


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
                _command.CommandText = "sp_AddChronicCondition";

                _command.Parameters.AddWithValue("@ConditionName", condition.ConditionName);
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
                _command.CommandText = "[sp_UpdateChronicCondition]";

                _command.Parameters.AddWithValue("@ConditionId", condition.ConditionId);
                _command.Parameters.AddWithValue("@ConditionName", condition.ConditionName);
                _command.Parameters.AddWithValue("@ConditionDescr", condition.ConditionDescr);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
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

        //**************** PATIENT CONDITION ******************


        public List<PatientCondition> GetPatientCondition(int patientId)
        {
            List<PatientCondition> conditionList = new List<PatientCondition>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetPatientCondition";

                _command.Parameters.AddWithValue("@PatientId", patientId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();
                

                while (dr.Read())
                {
                    PatientCondition condition = new PatientCondition();
                    condition.PatientConditionId = Convert.ToInt32(dr["PatientConditionId"]);
                    condition.ConditionName = dr["ConditionName"].ToString();
                    condition.ConditionDescr = dr["ConditionDescr"].ToString();


                    conditionList.Add(condition);
                }
                _connection.Close();
            }
            return conditionList;
        }
        public bool InsertPatientCondition(PatientCondition condition)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AddPatientCondition";

                _command.Parameters.AddWithValue("@PatientId", condition.PatientId);
                _command.Parameters.AddWithValue("@ConditionId", condition.ConditionId);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool UpdatePatientCondition(PatientCondition condition)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_UpdatePatientCondition]";

                _command.Parameters.AddWithValue("@PatientId", condition.PatientId);
                _command.Parameters.AddWithValue("@ConditionId", condition.ConditionId);
                _command.Parameters.AddWithValue("@PatientConditionId", condition.PatientConditionId);
                

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

       

        public bool DeletePatientCondition(PatientCondition condition)
        {
            int affectedRow = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_DeletePatientCondition ";

                _command.Parameters.AddWithValue("@PatientConditionId", condition.PatientConditionId);

                _connection.Open();
                affectedRow = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return affectedRow > 0 ? true : false;
        }
        public PatientCondition GetPatientConditionById(int id)
        {
            PatientCondition condition = new PatientCondition();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetPatientConditionById";

                _command.Parameters.AddWithValue("@PatientConditionId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    condition.PatientConditionId = Convert.ToInt32(dr["PatientConditionId"]);
                    condition.ConditionName = dr["ConditionName"].ToString();
                    condition.ConditionDescr = dr["ConditionDescr"].ToString();



                }
                _connection.Close();
            }
            return condition;
        }


        public User GetUserNameById(int id)
        {
            User user = new User();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetUserNameById";

                _command.Parameters.AddWithValue("@UserId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {


                    user.UserName = dr["UserName"].ToString();
                   



                }
                _connection.Close();
            }
            return user;
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
                    

                    nurseList.Add(nurse);
                }
                _connection.Close();
            }
            return nurseList;
        }


        //*****************CARE CONTRACT*************

        //Displaying All New Contracts
        public List<CareContract> GetPatientContract(int patientId)
        {

            
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNewContractByPatientId";

                _command.Parameters.AddWithValue("@PatientId", patientId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();
               

                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.StartDate = dr["StartDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["StartDate"]).Date;
                    contract.EndDate = dr["EndDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["EndDate"]).Date;
                    contract.Name = dr["FirstName"] == DBNull.Value ? "" : dr["FirstName"].ToString();
                    contract.Surname = dr["Surname"] == DBNull.Value ? "" : dr["Surname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.AddressLine2 = dr["AddressLine2"] == DBNull.Value ? "" : dr["AddressLine2"].ToString();
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();


                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public bool InsertContract(CareContract careContract)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_CreateCareContract";

                _command.Parameters.AddWithValue("@PatientId", careContract.PatientId);
                _command.Parameters.AddWithValue("@ContractDate", careContract.ContractDate);
                _command.Parameters.AddWithValue("@AddressLine1", careContract.AddressLine1);
                _command.Parameters.AddWithValue("@AddressLine2", careContract.AddressLine2);
                _command.Parameters.AddWithValue("@SuburbId", careContract.SuburbId);
                _command.Parameters.AddWithValue("@WoundDescription", careContract.WoundDescription);
                _command.Parameters.AddWithValue("@ContractStatus",careContract.Status);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool UpdatePatientContract(ContractVM contract)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpdateCareContractByPatient";

                _command.Parameters.AddWithValue("@PatientId", contract.PatientId);
                _command.Parameters.AddWithValue("@CareContractId", contract.ContractId);
                _command.Parameters.AddWithValue("@ContractDate", contract.ContractDate);
                _command.Parameters.AddWithValue("@SuburbId", contract.SuburbId);
                _command.Parameters.AddWithValue("@AddressLine1", contract.AddressLine1);
                _command.Parameters.AddWithValue("@AddressLine2", contract.AddressLine2);
                _command.Parameters.AddWithValue("@WoundDescription", contract.WoundDescription);
                _command.Parameters.AddWithValue("@ContractStatus", contract.Status);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public bool CancelContract(CareContract contract)
        {
            int affectedRow = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_DeleteCareContract]";

                _command.Parameters.AddWithValue("@CareContractId", contract.ContractId);
                _command.Parameters.AddWithValue("@Status", contract.Status);

                _connection.Open();
                affectedRow = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return affectedRow > 0 ? true : false;
        }

        public CareContract GetContractById(int id)
        {
            CareContract contract = new CareContract();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetCareContractById";

                _command.Parameters.AddWithValue("@ContractId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;                    
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.AddressLine2 = dr["AddressLine2"] == DBNull.Value ? "" : dr["AddressLine2"].ToString();                    
                    contract.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    contract.Status = dr["ContractStatus"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();


                }
                _connection.Close();
            }
            return contract;
        }

        public bool ContractExists(int patientId)
        {
            bool contractExists = false;

            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _connection.Open();
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure; 
                _command.CommandText = "sp_GetExistingContract";

                
                _command.Parameters.AddWithValue("@PatientId", patientId);

                int contractCount = Convert.ToInt32(_command.ExecuteScalar());

                contractExists = (contractCount > 0);
            }

            return contractExists;
        }

        public List<CareContract> GetNewNurseContract(int nurseId)
        {
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNewNurseContract";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["Firstname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.AddressLine1 = dr["AddressLine1"].ToString();                    
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();


                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public List<CareContract> GetAllNurseContracts(int nurseId)
        {
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNurseSuburb";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["Firstname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.AddressLine2 = dr["AddressLine2"].ToString();
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();


                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public CareContractVM GetNurseCareContractById(int id)
        {
            CareContractVM contract = new CareContractVM();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetNurseCareContractById]";

                _command.Parameters.AddWithValue("@CareContractId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.Status = dr["ContractStatus"].ToString();
                    if (!dr.IsDBNull(dr.GetOrdinal("StartDate")))
                    {
                        contract.StartDate = Convert.ToDateTime(dr["StartDate"]).Date;
                    }
                    else
                    {
                        contract.StartDate = DateTime.MinValue; 
                    }
                }
                _connection.Close();
            }
            return contract;
        }

        public bool NurseSelectContract(CareContractVM contract)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_NurseSelectContract";

                _command.Parameters.AddWithValue("@CareContactId", contract.ContractId);
                _command.Parameters.AddWithValue("@StartDate", contract.StartDate);
                _command.Parameters.AddWithValue("@NurseId", contract.NurseId);
                _command.Parameters.AddWithValue("@ContractStatus", contract.Status);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public List<CareContract> GetNurseAssignedContract(int nurseId)
        {
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetAssignedNurseContracts";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["Firstname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.StartDate = Convert.ToDateTime(dr["StartDate"]).Date;                
                    contract.AddressLine1 = dr["AddressLine1"].ToString();                    
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();


                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public List<CareContract> GetNurseClosedContract(int nurseId)
        {
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetAssignedClosedContracts]";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["Firstname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.StartDate = Convert.ToDateTime(dr["StartDate"]).Date;
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();


                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public int GetAssignedContractByNurseId(int userId)
        {
            int id = 0;

            try
            {
                using (_connection = new SqlConnection(GetConnectionString()))
                using (_command = _connection.CreateCommand())
                {
                    _connection.Open();
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = "sp_GetAssignedContractByNurseId";

                    _command.Parameters.AddWithValue("@NurseId", userId);


                    using (SqlDataReader dr = _command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return Convert.ToInt32(dr["ContractId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return id;
        }

        public List<Patient> sp_GetPatientsByAssignedNurse(int nurseId)
        {
            List<Patient> patients = new List<Patient>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetPatientsByAssignedNurse]";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    Patient patient = new Patient();
                    patient.PatientId = Convert.ToInt32(dr["PatientId"]);
                    patient.Name = dr["Firstname"].ToString();
                    patient.Surname = dr["Surname"].ToString();
                    patient.Gender = dr["Gender"].ToString();
                    patient.DoB = Convert.ToDateTime(dr["DateOfBirth"]).Date;
                    patient.ContactPerson = dr["EmergencyContactPerson"].ToString();
                    patient.ContactPersonNumber = dr["EmergencyContactNumber"].ToString();

                    patients.Add(patient);
                }
                _connection.Close();
            }
            return patients;
        }

       


        //Getting Contract By ID
        public CareContract GetContractToCloseByIditDetailsById(int id)
        {
            CareContract contract = new CareContract();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetContractToCloseById]";

                _command.Parameters.AddWithValue("@CareContractId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.StartDate = Convert.ToDateTime(dr["StartDate"]).Date;                   
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.AddressLine2 = dr["AddressLine2"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    

                }
                _connection.Close();
            }
            return contract;
        }
        public bool CloseContract(CareContract contract)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_CloseContract]";

                _command.Parameters.AddWithValue("@CareContractId", contract.ContractId);
                _command.Parameters.AddWithValue("@ContractStatus", contract.Status);
                _command.Parameters.AddWithValue("@EndDate", contract.EndDate);



                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }


        //************CARE VISIT*******

        public bool InsertCareVisit(CareVisitVM visit)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_AddCareVisit]";

                _command.Parameters.AddWithValue("@CareContractId", visit.ContractId);
                _command.Parameters.AddWithValue("@VisitDate", visit.VisitDate);
                _command.Parameters.AddWithValue("@ApproxArrivalTime", visit.ApproxArriveTime);
                

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public List<CareVisit> GetCareVisitByContractId(int contractId)
        {
            List<CareVisit> careVisits = new List<CareVisit>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetCareVisitsByContractId";

                _command.Parameters.AddWithValue("@CareContractId", contractId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareVisit visit = new CareVisit();
                    visit.VisitId = Convert.ToInt32(dr["CareVisitId"]);
                    visit.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    visit.VisitDate = Convert.ToDateTime(dr["VisitDate"]).Date;
                    visit.ApproxArriveTime = dr["ApproxArrivalTime"] != DBNull.Value? DateTime.Today.Add((TimeSpan)dr["ApproxArrivalTime"]) : (DateTime?)null;
                    visit.VisitArrivalTime = dr["VisitArrivalTime"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)dr["VisitArrivalTime"]) : (DateTime?)null;
                    visit.VisitDepartTime = dr["VisitDepartTime"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)dr["VisitDepartTime"]) : (DateTime?)null;
                    visit.WoundCondition = dr["WoundCondition"] != DBNull.Value ? dr["WoundCondition"].ToString() : null;
                    visit.Notes = dr["Notes"] != DBNull.Value ? dr["Notes"].ToString() : null;


                    careVisits.Add(visit);
                }
                _connection.Close();
            }
            return careVisits;
        }

        public VisitDetailsVM GetCareVisitDetailsById(int id)
        {
            VisitDetailsVM visit = new VisitDetailsVM();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetCareVisitDetailsById]";

                _command.Parameters.AddWithValue("@CareVisitId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    visit.VisitId = Convert.ToInt32(dr["CareVisitId"]);
                    visit.VisitArrivalTime = dr["VisitArrivalTime"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)dr["VisitArrivalTime"]) : (DateTime?)null;
                    visit.VisitDepartTime = dr["VisitDepartTime"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)dr["VisitDepartTime"]) : (DateTime?)null;
                    visit.WoundCondition = dr["WoundCondition"].ToString();
                    visit.Notes = dr["Notes"].ToString();
                }
                _connection.Close();
            }
            return visit;
        }


		public List<CareVisitDetailsVM> GetPatientCareVisit(int patientId)
		{
			List<CareVisitDetailsVM> visits = new List<CareVisitDetailsVM>();
			using (_connection = new SqlConnection(GetConnectionString()))
			{
				_command = _connection.CreateCommand();
				_command.CommandType = CommandType.StoredProcedure;
				_command.CommandText = "sp_GetPatientCareVisits";

				_command.Parameters.AddWithValue("@PatientId", patientId);
				_connection.Open();
				SqlDataReader dr = _command.ExecuteReader();


				while (dr.Read())
				{
					CareVisitDetailsVM visit = new CareVisitDetailsVM();
					visit.Surname = dr["Surname"].ToString();
					visit.Name = dr["Firstname"].ToString();
					if (!dr.IsDBNull(dr.GetOrdinal("Picture")))
					{
						visit.Picture = (byte[])dr["Picture"];
					}
					visit.VisitDate = Convert.ToDateTime(dr["ContractDate"]).Date;
					visit.ApproxArriveTime = Convert.ToDateTime(dr["ApproxArrivalTime"]);




					visits.Add(visit);
				}
				_connection.Close();
			}
			return visits;
		}

		public List<CareVisitDetailsVM> GetAllCareVisit()
		{
			List<CareVisitDetailsVM> visits = new List<CareVisitDetailsVM>();
			using (_connection = new SqlConnection(GetConnectionString()))
			{
				_command = _connection.CreateCommand();
				_command.CommandType = CommandType.StoredProcedure;
				_command.CommandText = "sp_GetAllCareVisits";

				
				_connection.Open();
				SqlDataReader dr = _command.ExecuteReader();


				while (dr.Read())
				{
					CareVisitDetailsVM visit = new CareVisitDetailsVM();
					visit.Surname = dr["Surname"].ToString();
					visit.Name = dr["Firstname"].ToString();
					visit.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
					visit.VisitDate = Convert.ToDateTime(dr["VisitDate"]).Date;
					visit.ApproxArriveTime = Convert.ToDateTime(dr["ApproxArrivalTime"]);
					visit.ArriveTime = Convert.ToDateTime(dr["VisitArrivalTime"]);
					visit.DepartTime = Convert.ToDateTime(dr["VisitDepartTime"]);
					visit.WoundCondition = dr["WoundCondition"].ToString();


					visits.Add(visit);
				}
				_connection.Close();
			}
			return visits;
		}



		public List<UpComingVisitVM> GetUpComingVisits(int nurseId, DateTime date)
        {
            List<UpComingVisitVM> visitList = new List<UpComingVisitVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpcomingCareVisits";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _command.Parameters.AddWithValue("@VisitDate", date);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    UpComingVisitVM visits = new UpComingVisitVM();
                    visits.CareVisitId = Convert.ToInt32(dr["CareVisitId"]);
                    visits.AddressLine1 = dr["AddressLine1"].ToString();
                    visits.SuburbName = dr["SuburbName"].ToString();
                    visits.ApproxArrivalTime = TimeSpan.Parse(dr["ApproxArrivalTime"].ToString());
                    visits.VisitDate = Convert.ToDateTime(dr["VisitDate"]).Date;


                    visitList.Add(visits);
                }
                _connection.Close();
            }
            return visitList;
        }

        public List<UpComingVisitVM> GetUpcomingVisitReport(int nurseId, DateTime date)
        {
            List<UpComingVisitVM> visit = new List<UpComingVisitVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_UpcomingCareVisits";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
                _command.Parameters.AddWithValue("@VisitDate", date);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    UpComingVisitVM visits = new UpComingVisitVM();
                    visits.CareVisitId = Convert.ToInt32(dr["CareVisitId"]);
                    visits.AddressLine1 = dr["AddressLine1"].ToString();
                    visits.SuburbName = dr["SuburbName"].ToString();
                    visits.ApproxArrivalTime = TimeSpan.Parse(dr["ApproxArrivalTime"].ToString());
                    visits.VisitDate = Convert.ToDateTime(dr["VisitDate"]).Date;

                    visit.Add(visits);
                }
                _connection.Close();
            }
            return visit;
        }

        public List<PatientConditionVM> GetPatientConditionReport(int nurseId)
        {
            List<PatientConditionVM> coniditions = new List<PatientConditionVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetPatientConditionReport";

                _command.Parameters.AddWithValue("@NurseId", nurseId);
               

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    PatientConditionVM condition = new PatientConditionVM();
                    condition.PatientId = Convert.ToInt32(dr["PatientId"]);
                    condition.Surname = dr["Surname"].ToString();
                    condition.Name = dr["Firstname"].ToString();
                    condition.ConditionName = dr["ConditionName"].ToString();
                    condition.ConditionDescr = dr["ConditionDescr"].ToString();

                    coniditions.Add(condition);
                }
                _connection.Close();
            }
            return coniditions;
        }



        //Updating Care Visit Details Based On Visit Carried Out
        public bool InsertCareVisitDetails(VisitDetailsVM visitDetails)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_InsertCareVisitDetails";

                _command.Parameters.AddWithValue("@CareVisitId", visitDetails.VisitId);
                _command.Parameters.AddWithValue("@VisitArrivalTime", visitDetails.VisitArrivalTime);
                _command.Parameters.AddWithValue("@VisitDepartTime", visitDetails.VisitDepartTime);
                _command.Parameters.AddWithValue("@WoundCondition", visitDetails.WoundCondition);
                _command.Parameters.AddWithValue("@Notes", visitDetails.Notes);

                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }


		//************OFFICE MANAGER***********

		public List<Patient> GetAllPatients()
		{
			List<Patient> patients = new List<Patient>();
			using (_connection = new SqlConnection(GetConnectionString()))
			{
				_command = _connection.CreateCommand();
				_command.CommandType = CommandType.StoredProcedure;
				_command.CommandText = "[sp_GetAllPatients]";

				
				_connection.Open();
				SqlDataReader dr = _command.ExecuteReader();


				while (dr.Read())
				{
					Patient patient = new Patient();
					patient.PatientId = Convert.ToInt32(dr["PatientId"]);
					patient.Surname = dr["Surname"].ToString();
					patient.Name = dr["FirstName"].ToString();
					patient.Gender = dr["Gender"].ToString();
					patient.DoB = Convert.ToDateTime(dr["DateOfBirth"]).Date;
					patient.ContactPerson = dr["EmergencyContactPerson"].ToString(); 
					patient.ContactPersonNumber = dr["EmergencyContactNumber"].ToString();

					patients.Add(patient);
				}
				_connection.Close();
			}
			return patients;
		}

        
        public List<Nurse> GetNurseByPreferredSuburb(int suburbId)
		{
			List<Nurse> nurses = new List<Nurse>();
			using (_connection = new SqlConnection(GetConnectionString()))
			{
				_command = _connection.CreateCommand();
				_command.CommandType = CommandType.StoredProcedure;
				_command.CommandText = "[sp_GetNursePreffSuburb]";

				_command.Parameters.AddWithValue("@SuburbId", suburbId);
				_connection.Open();
				SqlDataReader dr = _command.ExecuteReader();


				while (dr.Read())
				{
					Nurse nurse = new Nurse();
					nurse.NurseId = Convert.ToInt32(dr["NurseCode"]);
					nurse.Surname = dr["Surname"].ToString();
					nurse.Name = dr["FirstName"].ToString();
					nurse.Gender = dr["Gender"].ToString();
					

					nurses.Add(nurse);
				}
				_connection.Close();
			}
			return nurses;
		}

        public List<CareContract> GetNewContracts()
        {
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNewContract";

                
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.SuburbId = Convert.ToInt32(dr["SuburbId"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["Firstname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();

                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public List<CareContract> GetAllContracts()
        {
            List<CareContract> contractList = new List<CareContract>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetAllContracts";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    CareContract contract = new CareContract();
                    contract.ContractId = Convert.ToInt32(dr["CareContractId"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["Firstname"].ToString();
                    contract.Gender = dr["Gender"].ToString();
                    contract.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    contract.AddressLine1 = dr["AddressLine1"].ToString();
                    contract.SuburbName = dr["SuburbName"].ToString();
                    contract.WoundDescription = dr["WoundDescription"].ToString();
                    contract.Status = dr["ContractStatus"].ToString();

                    contractList.Add(contract);
                }
                _connection.Close();
            }
            return contractList;
        }

        public CareContract GetContractBySuburbId(int id)
        {
            CareContract contract = new CareContract();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetContractBySuburbId";

                _command.Parameters.AddWithValue("@SuburbId", id);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {

                    contract.ContractId = Convert.ToInt32(dr["ContractId"]);
                   
                }
                _connection.Close();
            }
            return contract;
        }

        public List<Nurse> GetNurseByContractSuburbId(int suburbId)
        {
            List<Nurse> nurseList = new List<Nurse>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetNurseByContractSuburb";

                _command.Parameters.AddWithValue("@SuburbId", suburbId);
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();


                while (dr.Read())
                {
                    Nurse contract = new Nurse();
                    contract.NurseId = Convert.ToInt32(dr["NurseCode"]);
                    contract.Surname = dr["Surname"].ToString();
                    contract.Name = dr["FirstName"].ToString();
                    contract.Gender = dr["Gender"].ToString();


                    nurseList.Add(contract);
                }
                _connection.Close();
            }
            return nurseList;
        }

        public bool AssignContract(CareContract contract)
        {
            int id = 0;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_AssignContract";

                _command.Parameters.AddWithValue("@CareContractId", contract.ContractId);
                _command.Parameters.AddWithValue("@Status", contract.Status);
                _command.Parameters.AddWithValue("@NurseId", contract.NurseId);
                



                _connection.Open();
                id = _command.ExecuteNonQuery();
                _connection.Close();
            }
            return id > 0 ? true : false;
        }

        public List<Nurse> GetAllNurses()
        {
            List<Nurse> nurses = new List<Nurse>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetAllNurses";

                
                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    Nurse nurse = new Nurse();
                    nurse.NurseId = Convert.ToInt32(dr["NurseCode"]);
                    nurse.Surname = dr["Surname"].ToString();
                    nurse.Name = dr["FirstName"].ToString();
                    nurse.Gender = dr["Gender"].ToString();

                    nurses.Add(nurse);
                }
                _connection.Close();
            }
            return nurses;
        }

        public List<PrefferedSuburb> GetNurseSuburb(int nurseId)
        {
            List<PrefferedSuburb> suburbs = new List<PrefferedSuburb>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetSuburbByNurseId]";


                _command.Parameters.AddWithValue("@NurseId", nurseId);

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    PrefferedSuburb suburb = new PrefferedSuburb();
                    suburb.PrefSuburbId = Convert.ToInt32(dr["PreferredSuburbId"]);
                    suburb.SuburbName = dr["SuburbName"].ToString();
                    suburb.CityName = dr["CityName"].ToString();


                    suburbs.Add(suburb);
                }
                _connection.Close();
            }
            return suburbs;
        }

        public List<NurseSuburbVM> GetNurseSuburbReport()
        {
            List<NurseSuburbVM> suburbs = new List<NurseSuburbVM>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[sp_GetNursesAndSuburbs]";


                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    NurseSuburbVM suburb = new NurseSuburbVM();
                    suburb.nurseId = Convert.ToInt32(dr["NurseCode"]);
                    suburb.Surname = dr["Surname"].ToString();
                    suburb.Name = dr["FirstName"].ToString();
                    suburb.Surburb = dr["SuburbName"].ToString();
                    


                    suburbs.Add(suburb);
                }
                _connection.Close();
            }
            return suburbs;
        }

        public List<ContractCareVisits> GetContractCareVisits()
        {
            List<ContractCareVisits> visits = new List<ContractCareVisits>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "sp_GetContractCareVisits";

                _connection.Open();
                SqlDataReader dr = _command.ExecuteReader();

                while (dr.Read())
                {
                    ContractCareVisits visit = new ContractCareVisits();
                    visit.Surname = dr["Surname"].ToString();
                    visit.Firstname = dr["Firstname"].ToString();
                    visit.AddressLine1 = dr["AddressLine1"].ToString();
                    visit.ContractDate = Convert.ToDateTime(dr["ContractDate"]).Date;
                    visit.VisitDate = Convert.ToDateTime(dr["VisitDate"]).Date;
                    visit.ApproxArrival = string.IsNullOrEmpty(dr["ApproxArrivalTime"].ToString()) ? TimeSpan.Zero : TimeSpan.Parse(dr["ApproxArrivalTime"].ToString());
                    visit.ArrivalTime = string.IsNullOrEmpty(dr["VisitArrivalTime"].ToString()) ? TimeSpan.Zero : TimeSpan.Parse(dr["VisitArrivalTime"].ToString());
                    visit.DepartTime = string.IsNullOrEmpty(dr["VisitDepartTime"].ToString()) ? TimeSpan.Zero : TimeSpan.Parse(dr["VisitDepartTime"].ToString());
                    visit.WoundCondition = dr["WoundCondition"].ToString();

                    visits.Add(visit);
                }
                _connection.Close();
            }
            return visits;
        }


    }
}
