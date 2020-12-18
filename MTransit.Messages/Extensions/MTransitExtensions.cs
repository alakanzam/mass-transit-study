﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MTransit.Messages.Interfaces;

namespace MTransit.Messages.Extensions
{
	public static class MTransitExtensions
	{
		#region Methods

		/// <summary>
		/// Register event processors.
		/// </summary>
		/// <typeparam name="TEvent"></typeparam>
		/// <param name="services"></param>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IServiceCollection AddEventProcessors<TEvent>(this IServiceCollection services,
			Assembly assembly)
		{
			var assemblyTypes = assembly.GetTypes();
			var processor = typeof(IEventProcessor<>).MakeGenericType(typeof(TEvent));

			var availableProcessorTypes = assemblyTypes
				.Where(x =>!x.IsInterface 
				           && x.GetInterfaces()
					           .Any(implementedInterface => implementedInterface == processor)
				           )
				.ToArray();

			foreach (var availableProcessorType in availableProcessorTypes)
				services.AddSingleton(processor, availableProcessorType);

			return services;
		}

		/// <summary>
		/// Register action processors.
		/// </summary>
		/// <typeparam name="TAction"></typeparam>
		/// <param name="services"></param>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IServiceCollection AddActionProcessors<TAction>(this IServiceCollection services,
			Assembly assembly)
		{
			var assemblyTypes = assembly.GetTypes();
			var processor = typeof(IActionProcessor<>).MakeGenericType(typeof(TAction));

			var availableProcessorTypes = assemblyTypes
				.Where(x => !x.IsInterface
				            && x.GetInterfaces()
					            .Any(implementedInterface => implementedInterface == processor)
				)
				.ToArray();

			foreach (var availableProcessorType in availableProcessorTypes)
				services.AddSingleton(processor, availableProcessorType);

			return services;
		}

		#endregion
	}
}