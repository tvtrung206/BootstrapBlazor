﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Components;

/// <summary>
/// QRCode 组件
/// </summary>
public partial class QRCode : IAsyncDisposable
{
    private ElementReference QRCodeElement { get; set; }

    private string? ClassString => CssBuilder.Default("qrcode")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ImageStyleString => $"width: {Width}px; height: {Width}px;";

    /// <summary>
    /// 获得/设置 二维码生成后回调委托
    /// </summary>
    [Parameter]
    public Func<Task>? OnGenerated { get; set; }

    /// <summary>
    /// 获得/设置 PlaceHolder 文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? PlaceHolder { get; set; }

    /// <summary>
    /// 获得/设置 是否显示工具按钮 默认 false
    /// </summary>
    [Parameter]
    public bool ShowButtons { get; set; }

    /// <summary>
    /// 获得/设置 清除按钮文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? ClearButtonText { get; set; }

    /// <summary>
    /// 获得/设置 生成按钮文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? GenerateButtonText { get; set; }

    /// <summary>
    /// 获得/设置 二维码内容信息
    /// </summary>
    [Parameter]
    public string? Content { get; set; }

    /// <summary>
    /// 获得/设置 二维码内容宽度 高度与宽度一致 最小值为 40 默认值 128
    /// </summary>
    [Parameter]
    public int Width { get; set; } = 128;

    [Inject]
    [NotNull]
    private IStringLocalizer<QRCode>? Localizer { get; set; }

    private string? MethodName { get; set; }

    [NotNull]
    private JSModule<QRCode>? Module { get; set; }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Width = Math.Max(40, Width);
        PlaceHolder ??= "Please input ...";
        ClearButtonText ??= "Clear";
        GenerateButtonText ??= "Generate";
    }

    /// <summary>
    /// OnParametersSetAsync 方法
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        await Generate();
    }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!string.IsNullOrEmpty(MethodName))
        {
            if (Module == null)
            {
                var jSObjectReference = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.BarCode/qrcode.bundle.min.js");
                Module = new JSModule<QRCode>(jSObjectReference, this);
            }
            await Module.InvokeVoidAsync("bb_qrcode", QRCodeElement, MethodName, Content ?? "");
            MethodName = null;
        }
    }

    private Task Clear()
    {
        MethodName = "clear";
        return Task.CompletedTask;
    }

    private Task Generate()
    {
        MethodName = string.IsNullOrEmpty(Content) ? "clear" : "generate";
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task Generated()
    {
        if (OnGenerated != null)
        {
            await OnGenerated.Invoke();
        }
    }

    /// <summary>
    /// Dispose 方法
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (Module != null)
            {
                await Module.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Dispose 方法
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}