﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Options.Recoveries
{
    public class BackupOptions
    {
        public string BackupFolderPath { get; set; }

        public string RestoreFolderPath { get; set; }

        public int MaximumObjects { get; set; } = 100;
    }
}
