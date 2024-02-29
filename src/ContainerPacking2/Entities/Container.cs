using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerPacking2.Entities
{
	/// <summary>
	/// The container to pack items into.
	/// </summary>
	public struct Container
	{
		#region Private Variables

		private long volume;

		#endregion Private Variables

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Container class.
		/// </summary>
		/// <param name="id">The container ID.</param>
		/// <param name="length">The container length.</param>
		/// <param name="width">The container width.</param>
		/// <param name="height">The container height.</param>
		public Container(int id, long length, long width, long height)
		{
			this.ID = id;
			this.Length = length;
			this.Width = width;
			this.Height = height;
			this.volume = length * width * height;
		}

		#endregion Constructors

		#region Public Properties
		/// <summary>
		/// Gets or sets the container ID.
		/// </summary>
		/// <value>
		/// The container ID.
		/// </value>
		public int ID { get; set; }

		/// <summary>
		/// Gets or sets the container length.
		/// </summary>
		/// <value>
		/// The container length.
		/// </value>
		public long Length { get; set; }

		/// <summary>
		/// Gets or sets the container width.
		/// </summary>
		/// <value>
		/// The container width.
		/// </value>
		public long Width { get; set; }

		/// <summary>
		/// Gets or sets the container height.
		/// </summary>
		/// <value>
		/// The container height.
		/// </value>
		public long Height { get; set; }

		/// <summary>
		/// Gets or sets the volume of the container.
		/// </summary>
		/// <value>
		/// The volume of the container.
		/// </value>
		public long Volume
		{
			get
			{
				return this.volume;
			}
			set
			{
				this.volume = value;
			}
		}

		#endregion Public Properties
	}
}
