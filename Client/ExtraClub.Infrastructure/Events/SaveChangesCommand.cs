﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ExtraClub.Infrastructure.Events
{
    public class SaveChangesCommand
    {
        public static readonly RoutedCommand Instance = new RoutedUICommand();
    }
}