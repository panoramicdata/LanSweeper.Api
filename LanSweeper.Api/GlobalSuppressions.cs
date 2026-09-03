// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Minor Code Smell",
	"S2333:Redundant modifiers should be removed",
	Justification = "The 'partial' modifier is required by the [GeneratedRegex] source generator, "
		+ "which emits the regex implementation into a second part of the type. Removing it "
		+ "breaks the build. The analyzer cannot see the generated part.",
	Scope = "type",
	Target = "~T:LanSweeper.Api.Infrastructure.LoggingHandler"
)]
