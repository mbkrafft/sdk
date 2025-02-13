// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine.Parsing;
using System.IO;
using Microsoft.Deployment.DotNet.Releases;
using Microsoft.DotNet.Cli;
using Microsoft.DotNet.Cli.Utils;
using Product = Microsoft.DotNet.Cli.Utils.Product;
using Microsoft.NET.Sdk.WorkloadManifestReader;
using System.Linq;

namespace Microsoft.DotNet.Workloads.Workload.Search
{
    internal class WorkloadSearchCommand : CommandBase
    {
        private readonly IReporter _reporter;
        private readonly VerbosityOptions _verbosity;
        private readonly IWorkloadResolver _workloadResolver;
        private readonly ReleaseVersion _sdkVersion;
        private readonly string _workloadIdStub;

        public WorkloadSearchCommand(
            ParseResult result,
            IReporter reporter = null,
            IWorkloadResolver workloadResolver = null,
            string version = null) : base(result)
        {
            _reporter = reporter ?? Reporter.Output;
            _verbosity = result.ValueForOption<VerbosityOptions>(WorkloadSearchCommandParser.VerbosityOption);
            _workloadIdStub = result.ValueForArgument<string>(WorkloadSearchCommandParser.WorkloadIdStubArgument);
            _sdkVersion = new ReleaseVersion(version ?? Product.Version) ?? new ReleaseVersion(result.ValueForOption<string>(WorkloadSearchCommandParser.VersionOption));
            var dotnetPath = Path.GetDirectoryName(Environment.ProcessPath);
            var workloadManifestProvider = new SdkDirectoryWorkloadManifestProvider(dotnetPath, _sdkVersion.ToString());
            _workloadResolver = workloadResolver ?? WorkloadResolver.Create(workloadManifestProvider, dotnetPath, _sdkVersion.ToString());
        }

        public override int Execute()
        {
            var avaliableWorkloads = _workloadResolver.GetAvaliableWorkloads();

            if (!string.IsNullOrEmpty(_workloadIdStub))
            {
                avaliableWorkloads = avaliableWorkloads.Where(workload => workload.Id.ToString().Contains(_workloadIdStub, StringComparison.OrdinalIgnoreCase));
            }

            var table = new PrintableTable<WorkloadDefinition>();
            table.AddColumn(LocalizableStrings.WorkloadIdColumnName, workload => workload.Id.ToString());
            table.AddColumn(LocalizableStrings.DescriptionColumnName, workload => workload.Description);
            if (_verbosity.VerbosityIsDetailedOrDiagnostic())
            {
                table.AddColumn(LocalizableStrings.IsAbstractColumnName, workload => workload.IsAbstract.ToString());
                table.AddColumn(LocalizableStrings.KindColumnName, workload => workload.Kind.ToString());
            }

            _reporter.WriteLine();
            table.PrintRows(avaliableWorkloads, l => _reporter.WriteLine(l));
            _reporter.WriteLine();

            return 0;
        }
    }
}
