// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;

namespace openrmf_ext_api_score.Models
{
    /// <summary>
    /// This is the class that shows the score of the STIG for all categories
    /// </summary>

    public class Score
    {

        public Score () { 
            created = DateTime.Now;
        }

        public string systemGroupId { get; set; }
        public string hostName { get; set;}
        public string stigType { get; set; }
        public string stigRelease { get; set; }
        public string title { get {
            string _title = string.Empty;
            _title = !string.IsNullOrEmpty(hostName) ? hostName.Trim() : "Unknown-Host";
            _title += "-" + stigType.Trim() + "-" + stigRelease.Trim();
            return _title;
        }}
        
        public DateTime created { get; set; }

        public int totalCat1Open { get; set; }
        public int totalCat1NotApplicable { get; set; }
        public int totalCat1NotAFinding { get; set; }
        public int totalCat1NotReviewed { get; set; }
        public int totalCat2Open { get; set; }
        public int totalCat2NotApplicable{ get; set; }
        public int totalCat2NotAFinding { get; set; }
        public int totalCat2NotReviewed { get; set; }
        public int totalCat3Open { get; set; }
        public int totalCat3NotApplicable { get; set; }
        public int totalCat3NotAFinding { get; set; }
        public int totalCat3NotReviewed { get; set; }

        public int totalOpen { get { return totalCat1Open + totalCat2Open + totalCat3Open;} }
        public int totalNotApplicable { get { return totalCat1NotApplicable + totalCat2NotApplicable + totalCat3NotApplicable;} }
        public int totalNotAFinding { get { return totalCat1NotAFinding + totalCat2NotAFinding + totalCat3NotAFinding;} }
        public int totalNotReviewed { get { return totalCat1NotReviewed + totalCat2NotReviewed + totalCat3NotReviewed;} }

        public int totalCat1 { get { return totalCat1NotAFinding + totalCat1NotApplicable + totalCat1NotReviewed + totalCat1Open;} }
        public int totalCat2 { get { return totalCat2NotAFinding + totalCat2NotApplicable + totalCat2NotReviewed + totalCat2Open;} }
        public int totalCat3 { get { return totalCat3NotAFinding + totalCat3NotApplicable + totalCat3NotReviewed + totalCat3Open;} }
    }
}