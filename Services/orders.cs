using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class orders
    {
        dbServices ds = new dbServices();
        public async Task<responseData> CreateOrder(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                // if (!rData.addInfo.ContainsKey("UserId") || !rData.addInfo.ContainsKey("CarId"))
                // {
                //     resData.rData["rCode"] = 4;
                //     resData.rData["rMessage"] = "Invalid input parameters!";
                //     return resData;
                // }

                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@CarId", rData.addInfo["CarId"])
                };

                var checkQuery = $"SELECT * FROM pc_student.CarsHeaven_Rentals WHERE UserId=@UserId AND CarId = @CarId;";
                var dbCheckData = await ds.ExecuteSQLAsync(checkQuery, checkParams);
                if (dbCheckData.Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Order already active!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@RentalStart", rData.addInfo["RentalStart"]),
                        new MySqlParameter("@RentalEnd", rData.addInfo["RentalEnd"]),
                        new MySqlParameter("@TotalPrice", rData.addInfo["TotalPrice"]),
                        new MySqlParameter("@PaymentStatus", rData.addInfo["PaymentStatus"]),
                        new MySqlParameter("@DriverId", rData.addInfo["DriverId"]),
                    };

                    var insertQuery = @"INSERT INTO pc_student.CarsHeaven_Rentals (CarId,UserId,RentalStart,RentalEnd,TotalPrice,PaymentStatus,DriverId) VALUES (@CarId,@UserId,@RentalStart,@RentalEnd,@TotalPrice,@PaymentStatus,@DriverId);";
                    var rowsAffected = await ds.ExecuteSQLAsync(insertQuery, insertParams);
                    if (rowsAffected.Count() == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Order created successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to place order!";
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

        public async Task<responseData> EditOrder(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@RentalId", rData.addInfo["RentalId"]),
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    // new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Rentals WHERE RentalId=@RentalId AND UserId=@UserId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "User orders not found!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@RentalId", rData.addInfo["RentalId"]),
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                        new MySqlParameter("@RentalStart", rData.addInfo["RentalStart"]),
                        new MySqlParameter("@RentalEnd", rData.addInfo["RentalEnd"]),
                        new MySqlParameter("@TotalPrice", rData.addInfo["TotalPrice"]),
                        new MySqlParameter("@PaymentStatus", rData.addInfo["PaymentStatus"]),
                        new MySqlParameter("@DriverId", rData.addInfo["DriverId"]),
                    };
                    var updatequery = @"UPDATE pc_student.CarsHeaven_Rentals SET CarId = @CarId, RentalStart=@RentalStart, RentalEnd=@RentalEnd, TotalPrice=@TotalPrice, PaymentStatus=@PaymentStatus, DriverId=@DriverId  
                                        WHERE RentalId = @RentalId AND UserId = @UserId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, failed to update Order!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Order updated successfully.";
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

        public async Task<responseData> DeleteOrder(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string RentalId = req.addInfo["RentalId"].ToString();


                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@RentalId", RentalId),
                    // new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.CarsHeaven_Rentals WHERE RentalId = @RentalId;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Order not found, Not deleted!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.CarsHeaven_Rentals WHERE RentalId = @RentalId;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == null)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to delete, Wrong order id!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Order deleted successfully";
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

        public async Task<responseData> GetOrderById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Car order found successfully!";
            try
            {
                string RentalId = req.addInfo["RentalId"].ToString();
                // string CarId = req.addInfo["CarId"].ToString();
                // string UserId = req.addInfo["UserId"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@RentalId", req.addInfo["RentalId"]),
                    // new MySqlParameter("@CarId", req.addInfo["CarId"]),
                    // new MySqlParameter("@UserId", req.addInfo["UserId"]),
                };

                string getsql = $"SELECT * FROM pc_student.CarsHeaven_Rentals " +
                             "WHERE RentalId = @RentalId;";
                var Orderdata = ds.ExecuteSQLName(getsql, myParams);
                if (Orderdata == null || Orderdata.Count == 0 || Orderdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car order not found!";
                }
                else
                {
                    var OrderData = Orderdata[0][0];
                    resData.rData["RentalId"] = OrderData["RentalId"];
                    resData.rData["CarId"] = OrderData["CarId"];
                    resData.rData["UserId"] = OrderData["UserId"];
                    resData.rData["RentalStart"] = OrderData["RentalStart"];
                    resData.rData["RentalEnd"] = OrderData["RentalEnd"];
                    resData.rData["TotalPrice"] = OrderData["TotalPrice"];
                    resData.rData["PaymentStatus"] = OrderData["PaymentStatus"];
                    resData.rData["DriverId"] = OrderData["DriverId"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct Order details!";
            }
            return resData;
        }

        public async Task<responseData> GetAllOrders(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.CarsHeaven_Rentals ORDER BY RentalId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get Ordered cars!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> OrdersList = new List<object>();
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
                                var Order = new
                                {
                                    RentalId = rowData.ElementAtOrDefault(0),
                                    CarId = rowData.ElementAtOrDefault(1),
                                    UserId = rowData.ElementAtOrDefault(2),
                                    RentalStart = rowData.ElementAtOrDefault(3),
                                    RentalEnd = rowData.ElementAtOrDefault(4),
                                    TotalPrice = rowData.ElementAtOrDefault(5),
                                    PaymentStatus = rowData.ElementAtOrDefault(6),
                                    DriverId = rowData.ElementAtOrDefault(7)
                                };
                                OrdersList.Add(Order);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All car orders successfully";
                resData.rData["Orders"] = OrdersList;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occurred: {ex.Message}";
            }
            return resData;
        }
    }
}