using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class resetPassword
    {
        dbServices ds = new dbServices();
        public async Task<responseData> SendOtpToPhone(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                };

                var sql = @"select * from pc_student.CarsHeaven_Users where Phone=@Phone;";
                var data = ds.ExecuteSQLName(sql, para);
                if (data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rStatus = 404;
                    resData.rData["rMessage"] = "Phone number not found!!";
                }
                else
                {
                    string otp = GenerateOTP();
                    MySqlParameter[] sessionParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@OTP", otp),
                        new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    };
                    string sessionQuery = $"INSERT INTO pc_student.CarsHeaven_Sessions (Phone, OTP) VALUES (@Phone, @OTP);";
                    ds.ExecuteSQLName(sessionQuery, sessionParams);

                    resData.eventID = req.eventID;
                    resData.rStatus = 200;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "OTP sent successfully!";
                    resData.rData["OTP"] = otp;
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;
        }
        public async Task<responseData> VerifyPhoneOtp(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    new MySqlParameter("@OTP", req.addInfo["OTP"].ToString())
                };

                var sql = @"SELECT * FROM pc_student.CarsHeaven_Sessions WHERE Phone=@Phone AND OTP=@OTP;";
                var data = ds.ExecuteSQLName(sql, para);

                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rStatus = 404;
                    resData.rData["rMessage"] = "Invalid OTP or OTP has expired.";
                }
                else
                {
                    resData.eventID = req.eventID;
                    resData.rStatus = 200;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "OTP verified successfully!";

                    MySqlParameter[] deleteParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                        new MySqlParameter("@OTP", req.addInfo["OTP"].ToString())
                    };
                    string deleteQuery = "DELETE FROM pc_student.CarsHeaven_Sessions WHERE Phone=@Phone AND OTP=@OTP;";
                    ds.ExecuteSQLName(deleteQuery, deleteParams);
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;
        }

        public async Task<responseData> ResetPasswordByPhone(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string Input = req.addInfo["Phone"].ToString();
                bool isMobileNumber = IsValidMobileNumber(Input);
                string columnName;
                if (isMobileNumber)
                {
                    columnName = "Phone";
                }
                else
                {
                    columnName = "";
                }

                string Phone = req.addInfo["Phone"].ToString();
                string NewPassword = req.addInfo["NewPassword"].ToString();
                string ConfirmPassword = req.addInfo["ConfirmPassword"].ToString();

                var para = new MySqlParameter[]
                {
                    new MySqlParameter("@Phone", Phone),
                    new MySqlParameter("@NewPassword", NewPassword)
                };

                var selectSql = $"SELECT * FROM pc_student.CarsHeaven_Users WHERE {columnName} = @Phone;";
                var data = ds.ExecuteSQLName(selectSql, para);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Phone number not found, Please enter a valid details";
                }
                else
                {
                    if (NewPassword == ConfirmPassword)
                    {
                        var updateSql = $"UPDATE pc_student.CarsHeaven_Users SET UserPassword = @NewPassword WHERE {columnName} = @Phone;";
                        var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                        if (rowsAffected == null)
                        {
                            selectSql = $"SELECT * FROM pc_student.CarsHeaven_Users WHERE {columnName} = @Phone AND UserPassword=@NewPassword;";
                            data = ds.ExecuteSQLName(selectSql, para);
                            if (data[0].Count() == 0)
                            {
                                resData.rData["rCode"] = 4;
                                resData.rData["rMessage"] = "Password already exist, new password must be different!";
                            }
                            else
                            {
                                resData.rData["rCode"] = 3;
                                resData.rData["rMessage"] = "Some error occured, could'nt reset password!";
                            }
                        }
                        else
                        {
                            resData.eventID = req.eventID;
                            resData.rData["rCode"] = 0;
                            resData.rData["rMessage"] = "Password reset successfully";
                        }
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "New password and confirm password must match!";
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

        public async Task<responseData> ResetPasswordByMail(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@email",req.addInfo["email"].ToString()),
                    new MySqlParameter("@newpassword",req.addInfo["newpassword"].ToString())
                };

                var sql = $"select * from SignUp where email=@email;";
                var check = ds.ExecuteSQLName(sql, para);
                if (check != null && check[0].Count() > 0)
                {
                    var query1 = $"update SignUp set password=@newpassword where email=@email";
                    var update = ds.executeSQL(query1, para);
                    if (update != null)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "New Password created Successfully";
                    }
                    else
                    {
                        resData.rData["rCode"] = 1;
                        resData.rData["rMessage"] = "Enter valid new password ";
                    }
                }
                else
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = " Oops... Error in Mail";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsValidMobileNumber(string phoneNumber)
        {
            string pattern = @"^[0-9]{7,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
        private string GenerateOTP()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }
}