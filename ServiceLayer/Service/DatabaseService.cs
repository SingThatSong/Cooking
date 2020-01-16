﻿using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer
{
    public sealed class DatabaseService
    {
        public static string DbFileName { get; set; } = "cooking.db";

        public static void InitDatabase()
        {
           using CookingContext context = new CookingContext(DbFileName);
           context.Database.Migrate();
        }
    }
}
