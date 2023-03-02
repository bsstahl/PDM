// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", 
    "CA1032:Implement standard exception constructors", 
    Justification = "It doesn't make sense for this type of Exception to have an InnerException since it is a validation type", 
    Scope = "type", 
    Target = "~T:PDM.Exceptions.WireTypeMismatchException")]
