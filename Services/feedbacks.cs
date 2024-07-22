using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class feedbacks
    {
        dbServices ds = new dbServices();
        public async Task<responseData> ContactUs(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.rData["rMessage"] = "Feedback sent successfully";
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    new MySqlParameter("@Topic", req.addInfo["Topic"].ToString()),
                    new MySqlParameter("@Message", req.addInfo["Message"].ToString()),
                    new MySqlParameter("@Address", req.addInfo["Address"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.CarsHeaven_Users WHERE Email=@Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult == null || checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Email not found in our records. Please register first!";
                }
                else
                {
                    var insertSql = $"INSERT INTO pc_student.CarsHeaven_Feedback (UserName, Email, Phone, Topic, Message, Address) VALUES(@UserName, @Email, @Phone, @Topic, @Message, @Address);";
                    var insertId = ds.ExecuteInsertAndGetLastId(insertSql, para);

                    if (insertId != 0)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Thank you for your response";
                    }
                    else
                    {
                        resData.rData["rCode"] = 1;
                        resData.rData["rMessage"] = "Failed to submit Feedback";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> DeleteFeedbackById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@FeedbackId", req.addInfo["FeedbackId"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString())
                };

                var checkSql = $"SELECT * FROM pc_student.CarsHeaven_Feedback WHERE FeedbackId=@FeedbackId OR Email=@Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Feedback not found, No records deleted!";
                }
                else
                {
                    var deleteSql = @"DELETE FROM pc_student.CarsHeaven_Feedback WHERE FeedbackId=@FeedbackId OR Email=@Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occurred, Feedback not deleted!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Feedback deleted successfully";
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

        public async Task<responseData> GetFeedbackById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Feedback details found successfully";
            try
            {
                string FeedbackId = req.addInfo["FeedbackId"].ToString();
                string Email = req.addInfo["Email"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@FeedbackId", FeedbackId),
                    new MySqlParameter("@Email", Email)
                };

                var getusersql = $"SELECT * FROM pc_student.CarsHeaven_Feedback WHERE FeedbackId=@FeedbackId OR Email=@Email;";
                var data = ds.ExecuteSQLName(getusersql, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed to get Feedback details!!";
                }
                else
                {
                    resData.rData["FeedbackId"] = data[0][0]["FeedbackId"];
                    resData.rData["UserName"] = data[0][0]["UserName"];
                    resData.rData["Phone"] = data[0][0]["Phone"];
                    resData.rData["Email"] = data[0][0]["Email"];
                    resData.rData["Topic"] = data[0][0]["Topic"];
                    resData.rData["Message"] = data[0][0]["Message"];
                    resData.rData["SentOn"] = data[0][0]["SentOn"];
                    resData.rData["ReadStatus"] = data[0][0]["ReadStatus"];
                    resData.rData["Address"] = data[0][0]["Address"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> GetAllFeedbacks(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.CarsHeaven_Feedback ORDER BY FeedbackId DESC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all Feedbacks!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> Contactslist = new List<object>();
                foreach (var rowSet in dbData)
                {
                    if (rowSet != null)
                    {
                        foreach (var row in rowSet)
                        {
                            if (row != null)
                            {
                                List<string> rowData = new List<string>();

                                foreach (var column in row)
                                {
                                    if (column != null)
                                    {
                                        rowData.Add(column.ToString());
                                    }
                                }
                                var Contact = new
                                {
                                    FeedbackId = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    CarId = rowData.ElementAtOrDefault(2),
                                    UserName = rowData.ElementAtOrDefault(3),
                                    Email = rowData.ElementAtOrDefault(4),
                                    Phone = rowData.ElementAtOrDefault(5),
                                    Address = rowData.ElementAtOrDefault(6),
                                    Topic = rowData.ElementAtOrDefault(7),
                                    Message = rowData.ElementAtOrDefault(8),
                                    Rating = rowData.ElementAtOrDefault(9),
                                    SentOn = rowData.ElementAtOrDefault(10),
                                    ReadStatus = rowData.ElementAtOrDefault(11),
                                };
                                Contactslist.Add(Contact);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All Feedbacks retrieved successfully";
                resData.rData["Contact"] = Contactslist;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }
    }
}