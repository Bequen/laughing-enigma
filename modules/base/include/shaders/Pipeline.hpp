#pragma once

#include <vulkan/vulkan_core.h>
#include <vector>
#include <optional>

#include "RenderContext.hpp"
#include "Shader.hpp"
#include "Gpu.hpp"

class Pipeline {
	VkPipeline m_pipeline;
    VkPipelineLayout m_layout;

public:
    Pipeline(VkPipelineLayout layout, VkPipeline pipeline) :
    m_layout(layout), m_pipeline(pipeline) {

    }

    VkPipeline pipeline() const {
        return m_pipeline;
    }

    VkPipelineLayout pipeline_layout() const {
        return m_layout;
    }

    Pipeline& use(RenderContext *pRenderContext) {
        vkCmdBindPipeline(pRenderContext->command_buffer(),
                          VK_PIPELINE_BIND_POINT_GRAPHICS,
                          m_pipeline);
        return *this;
    }

    Pipeline& bind_input_set(RenderContext *pRenderContext, uint32_t idx, uint32_t num, VkDescriptorSet *pSets) {
        vkCmdBindDescriptorSets(pRenderContext->command_buffer(),
                                VK_PIPELINE_BIND_POINT_GRAPHICS,
                                m_layout,
                                idx, num, pSets,
                                0, nullptr);

        return *this;
    }
};