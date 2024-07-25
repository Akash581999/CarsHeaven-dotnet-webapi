using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class editProfile
    {
        dbServices ds = new dbServices();
        public async Task<responseData> EditProfile(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@FirstName", req.addInfo["FirstName"].ToString()),
                    new MySqlParameter("@LastName", req.addInfo["LastName"].ToString()),
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    new MySqlParameter("@Address", req.addInfo["Address"].ToString()),
                };

                var updateSql = @"UPDATE pc_student.CarsHeaven_Users 
                                SET FirstName=@FirstName, LastName=@LastName, UserName = @UserName, Email = @Email, Phone = @Phone, Address = @Address 
                                WHERE UserId = @UserId";
                var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                if (rowsAffected != 0)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
                }
                else
                {
                    var selectSql = @"SELECT * FROM pc_student.CarsHeaven_Users WHERE UserId = @UserId";
                    var existingDataList = ds.ExecuteSQLName(selectSql, para);
                    if (existingDataList != null && existingDataList.Count > 0)
                    {

                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile updated successfully";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "No changes were made";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
        public async Task<responseData> EditProfilePic(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@ProfilePic", req.addInfo["ProfilePic"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.CarsHeaven_Users WHERE Email = @Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile pic not found, can not update!";
                }
                else
                {
                    string updateSql = $"UPDATE pc_student.CarsHeaven_Users SET ProfilePic = @ProfilePic WHERE Email = @Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                    if (rowsAffected == null)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Invalid credentials, Wrong Id or Password!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile Pic updated successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
        public async Task<responseData> DeleteProfilePic(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    // new MySqlParameter("@ProfilePic", req.addInfo["ProfilePic"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.CarsHeaven_Users WHERE Email = @Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile pic not found, Not removed!";
                }
                else
                {
                    var updateSql = $"UPDATE pc_student.CarsHeaven_Users SET ProfilePic = NULL WHERE Email = @Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                    if (rowsAffected == null)

                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to remove profile pic";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile pic removed Sucessfully";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}