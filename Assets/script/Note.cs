using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite donSprite;
    public Sprite katsuSprite;
    // ���̃m�[�c��ނɑΉ����邽�߂ɕK�v�ȃX�v���C�g��ǉ����Ă�������

    private float startTime;
    private int type;

    public void StartPlayback(float startTime, int type)
    {
        this.startTime = startTime;
        this.type = type;
        StartCoroutine(PlayNote());
    }

    private IEnumerator PlayNote()
    {
        // �m�[�c�̃A�j���[�V�����≹���Đ��Ȃǂ̏�����ǉ����Ă�������
        while (Time.time < startTime)
        {
            yield return null;
        }

        // �m�[�c�̕`��
        switch (type)
        {
            case 1:
                spriteRenderer.sprite = donSprite;
                break;
            case 2:
                spriteRenderer.sprite = katsuSprite;
                break;
                // ���̃m�[�c��ނɑΉ�����ꍇ�A�K�؂ȃX�v���C�g��ݒ肵�Ă�������
        }

        // �m�[�c�̍Đ������܂ł̑ҋ@
        // �K�؂Ȏ��Ԃ�A�j���[�V�����Ȃǂ�ǉ����Ă�������
        yield return new WaitForSeconds(1f);

        // �m�[�c�̍폜���\�����Ȃǂ̏�����ǉ����Ă�������
        Destroy(gameObject);
    }
}
