using System;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// In the event of a BeforeUnfold or AfterUnfold command that throws an error
    /// we re-package it and throw an instance of this class.
    /// 
    /// Part of the reason for this is that the *only* way for a template wizard
    /// to "cancel" the project unfolding (which you might want to do in a
    /// BeforeUnfold command) is to throw an error.
    /// </summary>
    internal class TemplateCommandFailedException : Exception
    {
        public TemplateCommandFailedException(Exception e)
            : base("Template Command Failed", e)
        {
        }
    }
}