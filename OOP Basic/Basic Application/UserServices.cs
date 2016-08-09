using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using NVCOSMO.Data.DTO.Response;
using NVCOSMO.Data.Entity;
using NVCOSMO.Data.Repository;
using NVCOSMO.Data.Repository.RepositoryContext;

namespace NVCOSMO.Process.Services
{
    public sealed class UserServices
    {
        #region Procedure Defination
        private const string PROC_USER_AUTH = "User_ByUsername @UserName";
        private const string PROC_GET_USER_BY_USERNAME_AND_PASSWORD = "User_ByUsernameAndPassword " +
                                                                     "@Username, @Password";
        private const string PROC_INSERT_USER = "User_InsertCommand_New @UserName, @Password, @LastLogin, " +
                                                "@UserTypeId, @TimeStamp, @UserIdStamp, @UserStatusId, " +
                                                "@IndividualId";
        private const string PROC_UPDATE_USER = "User_UpdateCommand @UserName, @Password, @LastLogin, " +
                                                "@UserTypeId, @TimeStamp, @UserIdStamp, @UserStatusId, " +
                                                "@IndividualId, @Id";

        private const string PROC_INDIVIDUAL_INFO_BY_SSN = "Individual_BySSN @SSN"; 
        #endregion

        private static EfRepository<UserEntity> _userRepository;

        public UserServices()
        {
        }

        private static SqlParameter PrepareSqlParam(string paramName, object paramValue)
        {
            #region Db Type Map

            Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>
            {
                [typeof (byte)] = DbType.Byte,
                [typeof (sbyte)] = DbType.SByte,
                [typeof (short)] = DbType.Int16,
                [typeof (ushort)] = DbType.UInt16,
                [typeof (int)] = DbType.Int32,
                [typeof (uint)] = DbType.UInt32,
                [typeof (long)] = DbType.Int64,
                [typeof (ulong)] = DbType.UInt64,
                [typeof (float)] = DbType.Single,
                [typeof (double)] = DbType.Double,
                [typeof (decimal)] = DbType.Decimal,
                [typeof (bool)] = DbType.Boolean,
                [typeof (string)] = DbType.String,
                [typeof (char)] = DbType.StringFixedLength,
                [typeof (Guid)] = DbType.Guid,
                [typeof (DateTime)] = DbType.DateTime,
                [typeof (DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof (byte[])] = DbType.Binary,
                [typeof (byte?)] = DbType.Byte,
                [typeof (sbyte?)] = DbType.SByte,
                [typeof (short?)] = DbType.Int16,
                [typeof (ushort?)] = DbType.UInt16,
                [typeof (int?)] = DbType.Int32,
                [typeof (uint?)] = DbType.UInt32,
                [typeof (long?)] = DbType.Int64,
                [typeof (ulong?)] = DbType.UInt64,
                [typeof (float?)] = DbType.Single,
                [typeof (double?)] = DbType.Double,
                [typeof (decimal?)] = DbType.Decimal,
                [typeof (bool?)] = DbType.Boolean,
                [typeof (char?)] = DbType.StringFixedLength,
                [typeof (Guid?)] = DbType.Guid,
                [typeof (DateTime?)] = DbType.DateTime,
                [typeof (DateTimeOffset?)] = DbType.DateTimeOffset
            };

            #endregion

            var dbType = typeMap[paramValue.GetType()];
            return new SqlParameter
            {
                ParameterName = paramName,
                Value = (object)paramValue ?? DBNull.Value,
                DbType = dbType
            };
        }

        public static OperationObjectResponse<Individual> GetIndividualIdBySsn(string ssn)
        {
            var individualRepository = new EfRepository<Individual>(new ObjectContext());
            if (string.IsNullOrEmpty(ssn))
                return new OperationObjectResponse<Individual>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(false, -3)
                };
            #region Prepare SQL Parameters
            object[] objParams =
            {
                PrepareSqlParam("SSN", ssn)
            };
            #endregion

            try
            {
                var individualInfo =
                    individualRepository.ExecuteStoredProcedureList<Individual>(
                        PROC_INDIVIDUAL_INFO_BY_SSN,
                        objParams);
                if (individualInfo == null || !individualInfo.Any())
                    return new OperationObjectResponse<Individual>
                    {
                        Object = null,
                        OperationResponse = new OperationResponse(false, -2)
                    };
                return new OperationObjectResponse<Individual>
                {
                    Object = individualInfo.FirstOrDefault(),
                    OperationResponse = new OperationResponse(true, 2)
                };
            }
            catch (Exception ex)
            {
                return new OperationObjectResponse<Individual>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(ex)
                };
            }
        }

        public static OperationObjectResponse<UserEntity> InsertUser(UserEntity userEntity)
        {
            _userRepository = new EfRepository<UserEntity>(new ObjectContext());
            if (userEntity == null)
                return new OperationObjectResponse<UserEntity>(null, false, -3);

            #region Prepare SQL Parameters
            object[] objParams =
            {
                PrepareSqlParam("UserName", userEntity.UserName),
                PrepareSqlParam("Password", userEntity.Password),
                PrepareSqlParam("LastLogin", DateTime.Now),
                PrepareSqlParam("UserTypeId", userEntity.UserTypeId),
                PrepareSqlParam("TimeStamp", DateTime.Now),
                PrepareSqlParam("UserIdStamp", userEntity.UserIdStamp),
                PrepareSqlParam("UserStatusId", userEntity.UserStatusId),
                PrepareSqlParam("IndividualId", userEntity.IndividualId)
            };
            #endregion

            try
            {
                var users = _userRepository.ExecuteStoredProcedureList<UserEntity>(PROC_INSERT_USER, objParams);
                if (users == null || !users.Any())
                    return new OperationObjectResponse<UserEntity>(null, false, -2);
                return new OperationObjectResponse<UserEntity>(users.FirstOrDefault(), true, 2);
            }
            catch (Exception ex)
            {
                return new OperationObjectResponse<UserEntity>(ex);
            }
        }

        public static OperationObjectResponse<UserEntity> UpdateUser(UserEntity userEntity)
        {
            _userRepository = new EfRepository<UserEntity>(new ObjectContext());
            if (userEntity == null)
                return new OperationObjectResponse<UserEntity>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(false, -3)
                };

            #region Prepare SQL Parameters
            object[] objParams =
            {
                PrepareSqlParam("UserName", userEntity.UserName),
                PrepareSqlParam("Password", userEntity.Password),
                PrepareSqlParam("LastLogin", userEntity.LastLogin),
                PrepareSqlParam("UserTypeId", userEntity.UserTypeId),
                PrepareSqlParam("TimeStamp", userEntity.TimeStamp),
                PrepareSqlParam("UserIdStamp", userEntity.UserIdStamp),
                PrepareSqlParam("UserStatusId", userEntity.UserStatusId),
                PrepareSqlParam("IndividualId", userEntity.IndividualId),
                PrepareSqlParam("Id", userEntity.Id)
            };
            #endregion

            try
            {
                var users = _userRepository.ExecuteStoredProcedureList<UserEntity>(PROC_UPDATE_USER, objParams);
                if (users == null || !users.Any())
                    return new OperationObjectResponse<UserEntity>
                    {
                        Object = null,
                        OperationResponse = new OperationResponse(false, -2)
                    };
                return new OperationObjectResponse<UserEntity>
                {
                    Object = users.FirstOrDefault(),
                    OperationResponse = new OperationResponse(true, 2)
                };
            }
            catch (Exception ex)
            {
                return new OperationObjectResponse<UserEntity>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(ex)
                };
            }
        }

        public static OperationListResponse<UserEntity> GetAllUserQueryable()
        {
            _userRepository = new EfRepository<UserEntity>(new ObjectContext());
            return new OperationListResponse<UserEntity>
            {
                RecordList = _userRepository.Table.ToList(),
                OperationResponse = new OperationResponse(true, 2)
            };
        }

        public static OperationObjectResponse<UserEntity> IsValidUser(string username, string password)
        {
            _userRepository = new EfRepository<UserEntity>(new ObjectContext());
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new OperationObjectResponse<UserEntity>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(false, -16)
                };

            #region Prepare SQL Parameters
            object[] objParams =
            {
                PrepareSqlParam("UserName", username),
                PrepareSqlParam("Password", password)
            };
            #endregion

            try
            {
                var users = _userRepository.ExecuteStoredProcedureList<UserEntity>(PROC_GET_USER_BY_USERNAME_AND_PASSWORD,
                    objParams);
                if (users == null || !users.Any())
                {
                    return new OperationObjectResponse<UserEntity>
                    {
                        Object = null,
                        OperationResponse = new OperationResponse(false, -2)
                    };
                }
                return new OperationObjectResponse<UserEntity>
                {
                    Object = users.FirstOrDefault(),
                    OperationResponse = new OperationResponse(true, 2)
                };
            }
            catch (Exception ex)
            {
                return new OperationObjectResponse<UserEntity>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(ex)
                };
            }
        }

        public static OperationObjectResponse<UserEntity> GetUserByUserEmail(string emailId)
        {
            _userRepository = new EfRepository<UserEntity>(new ObjectContext());
            if (string.IsNullOrEmpty(emailId))
                return new OperationObjectResponse<UserEntity>
                {
                    OperationResponse = new OperationResponse(false, -3),
                    Object = null
                };

            #region Prepare SQL Parameters

            object[] objParams =
            {
                PrepareSqlParam("UserName", emailId)
            };
            #endregion

            try
            {
                var users = _userRepository.ExecuteStoredProcedureList<UserEntity>(PROC_USER_AUTH, objParams);
                if (users == null || !users.Any())
                {
                    return new OperationObjectResponse<UserEntity>
                    {
                        OperationResponse = new OperationResponse(false, -2),
                        Object = null
                    };
                }
                return new OperationObjectResponse<UserEntity>
                {
                    Object = users.FirstOrDefault(),
                    OperationResponse = new OperationResponse(true, 2)
                };
            }
            catch (Exception ex)
            {
                return new OperationObjectResponse<UserEntity>
                {
                    Object = null,
                    OperationResponse = new OperationResponse(ex)
                };
            }
        }
    }
}
