// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.Workloads.Workload.Install;
using Microsoft.Extensions.EnvironmentAbstractions;

namespace Microsoft.DotNet.Cli.Workload.Install.Tests
{
    internal class MockWorkloadManifestUpdater : IWorkloadManifestUpdater
    {
        public int UpdateAdvertisingManifestsCallCount = 0;
        public int CalculateManifestUpdatesCallCount = 0;
        public int DownloadManifestPackagesCallCount = 0;
        public int ExtractManifestPackagesToTempDirCallCount = 0;
        private IEnumerable<(ManifestId, ManifestVersion, ManifestVersion)> _manifestUpdates;
        private string _tempDirManifestPath;

        public MockWorkloadManifestUpdater(IEnumerable<(ManifestId, ManifestVersion, ManifestVersion)> manifestUpdates = null, string tempDirManifestPath = null)
        {
            _manifestUpdates = manifestUpdates ?? new List<(ManifestId, ManifestVersion, ManifestVersion)>();
            _tempDirManifestPath = tempDirManifestPath;
        }

        public Task UpdateAdvertisingManifestsAsync(bool includePreview, DirectoryPath? cachePath = null)
        {
            UpdateAdvertisingManifestsCallCount++;
            return Task.CompletedTask;
        }

        public IEnumerable<(ManifestId, ManifestVersion, ManifestVersion)> CalculateManifestUpdates()
        {
            CalculateManifestUpdatesCallCount++;
            return _manifestUpdates;
        }

        public Task<IEnumerable<string>> DownloadManifestPackagesAsync(bool includePreviews, DirectoryPath downloadPath)
        {
            DownloadManifestPackagesCallCount++;
            return Task.FromResult(new List<string>() as IEnumerable<string>);
        }

        public Task ExtractManifestPackagesToTempDirAsync(IEnumerable<string> manifestPackages, DirectoryPath tempDir)
        {
            ExtractManifestPackagesToTempDirCallCount++;
            if (!string.IsNullOrEmpty(_tempDirManifestPath))
            {
                Directory.CreateDirectory(Path.Combine(tempDir.Value, "mockPath"));
                File.Copy(_tempDirManifestPath, Path.Combine(tempDir.Value, "mockPath", "WorkloadManifest.json"));
            }
            return Task.CompletedTask;
        }

        public IEnumerable<string> GetManifestPackageUrls(bool includePreviews)
        {
            return new string[] { "mock-manifest-url" };
        }
    }
}
