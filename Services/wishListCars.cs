using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class wishListCars
    {
        dbServices ds = new dbServices();
        public async Task<responseData> AddToWishList(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                if (!rData.addInfo.ContainsKey("UserId") || !rData.addInfo.ContainsKey("CarId"))
                {
                    resData.rData["rCode"] = 4;
                    resData.rData["rMessage"] = "Invalid input parameters!";
                    return resData;
                }

                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@CarId", rData.addInfo["CarId"])
                };

                var checkQuery = @"SELECT COUNT(*) FROM pc_student.CarsHeaven_Wishlist WHERE UserId=@UserId AND CarId = @CarId;";
                var dbCheckData = await ds.ExecuteSQLAsync(checkQuery, checkParams);
                if (dbCheckData.Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car with this Id already exists in wishlist!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                    };

                    var insertQuery = @"INSERT INTO pc_student.CarsHeaven_Wishlist (UserId, CarId) VALUES (@UserId, @CarId);";
                    var rowsAffected = await ds.ExecuteSQLAsync(insertQuery, insertParams);
                    if (rowsAffected.Count() == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car added to wishlist successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add car into wishlist!";
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

        public async Task<responseData> UpdateInWishList(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@WishlistId", rData.addInfo["WishlistId"]),
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Wishlist WHERE WishlistId=@WishlistId AND UserId=@UserId AND CarId=@CarId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found in wishlist!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@WishlistId", rData.addInfo["WishlistId"]),
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                   };
                    var updatequery = @"UPDATE pc_student.CarsHeaven_Wishlist SET CarId = @CarId WHERE UserId = @UserId AND WishlistId = @WishlistId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, failed to update wishlist!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Wishlist updated successfully.";
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

        public async Task<responseData> RemoveFromWishList(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string WishlistId = rData.addInfo["WishlistId"].ToString();
                string UserId = rData.addInfo["UserId"].ToString();
                string CarId = rData.addInfo["CarId"].ToString();

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@WishlistId", WishlistId),
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@CarId", CarId)
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Wishlist WHERE WishlistId = @WishlistId AND UserId=@UserId AND CarId = @CarId;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData.Count == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found in the wishlist!";
                }
                else
                {
                    para = new MySqlParameter[]
                    {
                        new MySqlParameter("@WishlistId", WishlistId),
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@CarId", CarId)
                    };

                    var deleteSql = @"DELETE FROM pc_student.CarsHeaven_Wishlist WHERE WishlistId = @WishlistId AND UserId = @UserId AND CarId = @CarId;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected > 0)
                    {
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car removed from wishlist successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Failed to remove car from wishlist!";
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

        public async Task<responseData> GetAWishListCar(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Car found successfully!";
            try
            {
                string WishlistId = req.addInfo["WishlistId"].ToString();
                string UserId = req.addInfo["UserId"].ToString();
                string CarId = req.addInfo["CarId"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@WishlistId", req.addInfo["WishlistId"]),
                    new MySqlParameter("@UserId", req.addInfo["UserId"]),
                    new MySqlParameter("@CarId", req.addInfo["CarId"])
                };

                string getsql = $"SELECT * FROM pc_student.CarsHeaven_Wishlist " +
                             "WHERE WishlistId = @WishlistId AND UserId = @UserId AND CarId = @CarId;";
                var Wishlistdata = ds.ExecuteSQLName(getsql, myParams);
                if (Wishlistdata == null || Wishlistdata.Count == 0 || Wishlistdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found!";
                }
                else
                {
                    var WishlistData = Wishlistdata[0][0];
                    resData.rData["WishlistId"] = WishlistData["WishlistId"];
                    resData.rData["UserId"] = WishlistData["UserId"];
                    resData.rData["CarId"] = WishlistData["CarId"];
                    resData.rData["DateAdded"] = WishlistData["DateAdded"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct wishlist details!";
            }
            return resData;
        }

        public async Task<responseData> GetAllWishListCars(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.CarsHeaven_Wishlist ORDER BY WishlistId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get wishlist cars!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> WishlistsList = new List<object>();
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
                                var Wishlist = new
                                {
                                    WishlistId = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    CarId = rowData.ElementAtOrDefault(2),
                                    Quantity = rowData.ElementAtOrDefault(3),
                                    Price = rowData.ElementAtOrDefault(4),
                                    TotalPrice = rowData.ElementAtOrDefault(5)
                                };
                                WishlistsList.Add(Wishlist);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All cars found in wishlist successfully";
                resData.rData["Wishlists"] = WishlistsList;
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