﻿using MongoDB.Bson;

namespace LetPortal.Core.Utils
{
    public class DataUtil
    {
        public static string GenerateUniqueId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}