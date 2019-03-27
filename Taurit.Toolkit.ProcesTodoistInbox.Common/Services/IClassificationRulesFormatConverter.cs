﻿using System;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public interface IClassificationRulesFormatConverter
    {
        ClassificationRule Convert(String conciseRule);
    }
}