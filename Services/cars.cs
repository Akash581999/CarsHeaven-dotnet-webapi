using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class cars
    {
        dbServices ds = new dbServices();

        public async Task<responseData> AddCar(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@CarName", rData.addInfo["CarName"]),
                };

                var checkQuery = @"SELECT * FROM pc_student.CarsHeaven_Cars WHERE CarName = @CarName;";
                var dbCheckData = ds.ExecuteSQLName(checkQuery, checkParams);
                if (dbCheckData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car with this name already exists!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@BrandName", rData.addInfo["BrandName"]),
                        new MySqlParameter("@CarName", rData.addInfo["CarName"]),
                        new MySqlParameter("@CarType", rData.addInfo["CarType"]),
                        new MySqlParameter("@CarPic", rData.addInfo["CarPic"]),
                        new MySqlParameter("@Color", rData.addInfo["Color"]),
                        new MySqlParameter("@Seats", rData.addInfo["Seats"]),
                        new MySqlParameter("@RentRate", rData.addInfo["RentRate"]),
                        new MySqlParameter("@Mileage", rData.addInfo["Mileage"]),
                        new MySqlParameter("@FuelType", rData.addInfo["FuelType"]),
                        new MySqlParameter("@Transmission", rData.addInfo["Transmission"]),
                        new MySqlParameter("@Description", rData.addInfo["Description"]),
                        new MySqlParameter("@AvailabilityStatus", rData.addInfo["AvailabilityStatus"]),
                        new MySqlParameter("@RegistrationNumber", rData.addInfo["RegistrationNumber"]),
                        new MySqlParameter("@YearOfManufacture", rData.addInfo["YearOfManufacture"]),
                        new MySqlParameter("@InsuranceDetails", rData.addInfo["InsuranceDetails"]),
                        new MySqlParameter("@LocationId", rData.addInfo["LocationId"]),
                    };
                    var insertQuery = @"INSERT INTO pc_student.CarsHeaven_Cars (BrandName, CarName, CarType, CarPic, Color, Seats, RentRate, Mileage, FuelType,Transmission, Description, AvailabilityStatus, RegistrationNumber, YearOfManufacture, InsuranceDetails, LocationId) 
                                        VALUES (@BrandName, @CarName, @CarType, @CarPic, @Color, @Seats, @RentRate, @Mileage, @FuelType, @Transmission, @Description, @AvailabilityStatus, @RegistrationNumber, @YearOfManufacture, @InsuranceDetails, @LocationId);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);

                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car added successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add car!";
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


        public async Task<responseData> EditCar(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Cars WHERE CarId=@CarId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No car found with the provided Id!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@CarId", rData.addInfo["CarId"]),
                        new MySqlParameter("@BrandName", rData.addInfo["BrandName"]),
                        new MySqlParameter("@CarName", rData.addInfo["CarName"]),
                        new MySqlParameter("@CarType", rData.addInfo["CarType"]),
                        new MySqlParameter("@CarPic", rData.addInfo["CarPic"]),
                        new MySqlParameter("@Color", rData.addInfo["Color"]),
                        new MySqlParameter("@Seats", rData.addInfo["Seats"]),
                        new MySqlParameter("@RentRate", rData.addInfo["RentRate"]),
                        new MySqlParameter("@Mileage", rData.addInfo["Mileage"]),
                        new MySqlParameter("@FuelType", rData.addInfo["FuelType"]),
                        new MySqlParameter("@Transmission", rData.addInfo["Transmission"]),
                        new MySqlParameter("@Description", rData.addInfo["Description"]),
                        new MySqlParameter("@AvailabilityStatus", rData.addInfo["AvailabilityStatus"]),
                        new MySqlParameter("@RegistrationNumber", rData.addInfo["RegistrationNumber"]),
                        new MySqlParameter("@YearOfManufacture", rData.addInfo["YearOfManufacture"]),
                        new MySqlParameter("@InsuranceDetails", rData.addInfo["InsuranceDetails"]),
                        new MySqlParameter("@LocationId", rData.addInfo["LocationId"]),
                   };
                    var updatequery = @"UPDATE pc_student.CarsHeaven_Cars
                                        SET BrandName=@BrandName, CarName = @CarName, CarType = @CarType, CarPic = @CarPic, Color = @Color, Seats = @Seats, RentRate=@RentRate, Mileage=@Mileage, FuelType=@FuelType, Transmission=@Transmission, Description=@Description, AvailabilityStatus=@AvailabilityStatus, RegistrationNumber=@RegistrationNumber, YearOfManufacture=@YearOfManufacture, InsuranceDetails=@InsuranceDetails, LocationId=@LocationId 
                                        WHERE CarId = @CarId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, couldn't update details!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car details updated successfully.";
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

        public async Task<responseData> DeleteCar(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@CarId", rData.addInfo["CarId"].ToString()),
                    new MySqlParameter("@BrandName", rData.addInfo["BrandName"].ToString()),
                    new MySqlParameter("@CarName", rData.addInfo["CarName"].ToString())
                };

                var query = @"SELECT * FROM pc_student.CarsHeaven_Cars WHERE CarId=@CarId OR BrandName=@BrandName OR CarName=@CarName;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.CarsHeaven_Cars WHERE CarId=@CarId OR BrandName=@BrandName OR CarName=@CarName;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Car deleted successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Couldn't delete car!";
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

        public async Task<responseData> GetCar(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Car found successfully!";
            try
            {
                string CarId = req.addInfo["CarId"].ToString();
                string BrandName = req.addInfo["BrandName"].ToString();
                string CarName = req.addInfo["CarName"].ToString();
                string CarType = req.addInfo["CarType"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@CarId", req.addInfo["CarId"]),
                    new MySqlParameter("@BrandName", req.addInfo["BrandName"]),
                    new MySqlParameter("@CarName", req.addInfo["CarName"]),
                    new MySqlParameter("@CarType", req.addInfo["CarType"])
                };

                string getsql = $"SELECT * FROM pc_student.CarsHeaven_Cars " +
                             "WHERE CarId = @CarId OR BrandName=@BrandName OR CarName = @CarName OR CarType = @CarType;";
                var carsdata = ds.ExecuteSQLName(getsql, myParams);
                if (carsdata == null || carsdata.Count == 0 || carsdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Car not found!";
                }
                else
                {
                    var carsData = carsdata[0][0];
                    resData.rData["CarId"] = carsData["CarId"];
                    resData.rData["BrandName"] = carsData["BrandName"];
                    resData.rData["CarName"] = carsData["CarName"];
                    resData.rData["CarType"] = carsData["CarType"];
                    resData.rData["CarPic"] = carsData["CarPic"];
                    resData.rData["Color"] = carsData["Color"];
                    resData.rData["Seats"] = carsData["Seats"];
                    resData.rData["RentRate"] = carsData["RentRate"];
                    resData.rData["Mileage"] = carsData["Mileage"];
                    resData.rData["FuelType"] = carsData["FuelType"];
                    resData.rData["Transmission"] = carsData["Transmission"];
                    resData.rData["Description"] = carsData["Description"];
                    resData.rData["AvailabilityStatus"] = carsData["AvailabilityStatus"];
                    resData.rData["RegistrationNumber"] = carsData["RegistrationNumber"];
                    resData.rData["YearOfManufacture"] = carsData["YearOfManufacture"];
                    resData.rData["InsuranceDetails"] = carsData["InsuranceDetails"];
                    resData.rData["LocationId"] = carsData["LocationId"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct car details!";
            }
            return resData;
        }

        public async Task<responseData> GetAllCars(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.CarsHeaven_Cars ORDER BY CarId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all cars!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> CarsList = new List<object>();
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
                                var cars = new
                                {
                                    CarId = rowData.ElementAtOrDefault(0),
                                    BrandName = rowData.ElementAtOrDefault(1),
                                    CarName = rowData.ElementAtOrDefault(2),
                                    CarType = rowData.ElementAtOrDefault(3),
                                    CarPic = rowData.ElementAtOrDefault(4),
                                    Color = rowData.ElementAtOrDefault(5),
                                    Seats = rowData.ElementAtOrDefault(6),
                                    RentRate = rowData.ElementAtOrDefault(7),
                                    Mileage = rowData.ElementAtOrDefault(8),
                                    FuelType = rowData.ElementAtOrDefault(9),
                                    Transmission = rowData.ElementAtOrDefault(10),
                                    Description = rowData.ElementAtOrDefault(11),
                                    AvailabilityStatus = rowData.ElementAtOrDefault(12),
                                    RegistrationNumber = rowData.ElementAtOrDefault(13),
                                    YearOfManufacture = rowData.ElementAtOrDefault(14),
                                    InsuranceDetails = rowData.ElementAtOrDefault(15),
                                    LocationId = rowData.ElementAtOrDefault(16),
                                };
                                CarsList.Add(cars);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All cars found successfully";
                resData.rData["carss"] = CarsList;
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
