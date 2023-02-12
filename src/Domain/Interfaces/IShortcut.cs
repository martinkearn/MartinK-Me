using System;
namespace Domain.Interfaces
{
    /// <summary>
    /// An interface to define a Shortcut
    /// </summary>
	public interface IShortcut
	{
        /// <summary>
        /// The unique ID and Azure Storage Table entity key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Text representation of the shortcut Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Title of the shortcut
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Which group the shortcut belongs to
        /// </summary>
        public string Group { get; set; }
    }
}

