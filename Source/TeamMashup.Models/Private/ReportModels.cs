using System;
using System.Collections.Generic;
using TeamMashup.Models.Admin;

namespace TeamMashup.Models.Private
{
    public class BurndownModel
    {
        public IList<DataSet> DataSets { get; set; }

        public BurndownModel()
        {
            DataSets = new List<DataSet>();
        }
    }

    public class DataSet
    {
        public string Key { get; set; }

        public string Label { get; set; }

        public string Data { get; set; }
    }

    public class IterationReportModel
    {
        public long IterationId { get; set; }

        public int Commited { get; set; }

        public int Completed { get; set; }

        public IList<Tuple<UserModel, IterationIssueStats>> UserStats { get; set; }

        public IterationReportModel()
        {
            UserStats = new List<Tuple<UserModel, IterationIssueStats>>();
        }
    }

    public class IterationIssueStats
    {
        public int DefinedPoints { get; set; }

        public int InProgressPoints { get; set; }

        public int DonePoints { get; set; }
    }
}