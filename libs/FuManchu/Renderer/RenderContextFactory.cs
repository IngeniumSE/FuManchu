// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System;

using FuManchu.Binding;

/// <summary>
/// Provides factory methods for creating <see cref="RenderContext"/> instances.
/// </summary>
public static class RenderContextFactory
{
	/// <summary>
	/// Creates a <see cref="RenderContext"/> using the renderer provided.
	/// </summary>
	/// <param name="renderer">The renderering parser visitor.</param>
	/// <param name="model">The model.</param>
	/// <param name="unknownValueResolver">The value resolver for handling unknown values.</param>
	/// <returns>The render context.</returns>
	public static RenderContext CreateRenderContext(
		RenderingParserVisitor renderer,
		object? model = null,
		UnknownValueResolver? unknownValueResolver = null)
	{
		if (renderer == null)
		{
			throw new ArgumentNullException("renderer");
		}

		var context = new RenderContext(renderer)
		{
			TemplateData = new TemplateData()
			{
				Model = model,
				ModelMetadata = (model == null) ? null : renderer.ModelMetadataProvider.GetMetadataForType(() => model, model.GetType())
			},
			ModelMetadataProvider = renderer.ModelMetadataProvider,
			Service = renderer.Service,
			UnknownValueResolver = unknownValueResolver
		};

		return context;
	}

	/// <summary>
	/// Creates a child <see cref="RenderContext"/> based on the parent context provided.
	/// </summary>
	/// <param name="parent">The parent render context.</param>
	/// <param name="model">The child model.</param>
	/// <returns>The parent render context.</returns>
	public static RenderContext CreateRenderContext(RenderContext parent, object? model = null)
	{
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}

		model = model ?? parent.TemplateData.Model;

		var context = new RenderContext(parent.Visitor, parent)
		{
			TemplateData = new TemplateData()
			{
				Model = model,
				ModelMetadata = (model == null) ? null : parent.ModelMetadataProvider.GetMetadataForType(() => model, model.GetType())
			},
			ModelMetadataProvider = parent.ModelMetadataProvider,
			Service = parent.Service,
			UnknownValueResolver = parent.UnknownValueResolver
		};

		// Set the root context 
		context.RootRenderContext = parent.RootRenderContext ?? parent;

		return context;
	}
}
