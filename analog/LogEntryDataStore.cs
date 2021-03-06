﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analog
{
    public class LogEntryDataStore
    {
        private string _databaseFilename = "tmp.db";
        private string _connectionString = null;

        public LogEntryDataStore()
        {
            // Set up connection string using various pragmas for insert performance
            var connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = _databaseFilename;
            connectionStringBuilder.Version = 3;
            connectionStringBuilder.PageSize = 4096;
            connectionStringBuilder.JournalMode = SQLiteJournalModeEnum.Memory;
            connectionStringBuilder.SyncMode = SynchronizationModes.Off;

            _connectionString = connectionStringBuilder.ToString();
        }

        public void CreateDatabaseIfNeeded()
        {
            if (!File.Exists(_databaseFilename))
            {
                SQLiteConnection.CreateFile(_databaseFilename);
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    new SQLiteCommand(File.ReadAllText("schema.sql"), conn).ExecuteNonQuery();
                }
            }
        }

        public void ClearDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                new SQLiteCommand("DELETE FROM entries", conn).ExecuteNonQuery();
            }
        }

        public int PopulateDatabaseFromFiles(string[] files)
        {
            ClearDatabase();

            var resultCount = 0;

            var excludeStaticAssetRequests = true;

            var sql = @"INSERT INTO entries VALUES (?,?,?,?,?,?,?,?,?);";

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;

                        var parameters = new SQLiteParameter[] {
                            new SQLiteParameter("@datetime", DbType.String),
                            new SQLiteParameter("@ip", DbType.String),
                            new SQLiteParameter("@status", DbType.Int32),
                            new SQLiteParameter("@method", DbType.String),
                            new SQLiteParameter("@url", DbType.String),
                            new SQLiteParameter("@query", DbType.String),
                            new SQLiteParameter("@useragent", DbType.String),
                            new SQLiteParameter("@bytesout", DbType.Int32),
                            new SQLiteParameter("@bytesin", DbType.Int32)
                        };

                        cmd.Parameters.AddRange(parameters);

                        foreach (var file in files)
                        {
                            var lines = File.ReadLines(file);

                            foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#")))
                            {
                                var vals = line.Split(' ');

                                if (!excludeStaticAssetRequests || ProcessLine(vals))
                                {
                                    parameters[0].Value = vals[0] + " " + vals[1];
                                    parameters[1].Value = vals[7];
                                    parameters[2].Value = vals[11];
                                    parameters[3].Value = vals[3];
                                    parameters[4].Value = vals[4];
                                    parameters[5].Value = vals[5] == "-" ? null : vals[5];
                                    parameters[6].Value = vals[8];
                                    parameters[7].Value = vals[12];
                                    parameters[8].Value = vals[13];

                                    cmd.ExecuteNonQuery();

                                    resultCount++;
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
            }

            return resultCount;
        }

        public DataTable Query(string query)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = query;

                using (var a = new SQLiteDataAdapter(cmd))
                {
                    var table = new DataTable();
                    a.Fill(table);
                    return table;
                }
            }
        }

        private bool ProcessLine(string[] values)
        {
            return !values[4].EndsWith(".jpg")
                && !values[4].EndsWith(".png")
                && !values[4].EndsWith(".js")
                && !values[4].EndsWith(".woff")
                && !values[4].EndsWith(".woff2")
                && !values[4].EndsWith(".gif");
        }
    }
}
