import { fakeAsync, tick } from '@angular/core/testing';
import { ToastService } from './toast.service';

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    service = new ToastService();
  });

  it('starts empty', () => {
    expect(service.toasts()).toEqual([]);
  });

  it('adds a toast with the requested kind and message', () => {
    service.success('Saved.');

    expect(service.toasts().length).toBe(1);
    expect(service.toasts()[0].kind).toBe('success');
    expect(service.toasts()[0].message).toBe('Saved.');
  });

  it('keeps multiple toasts stacked in order', () => {
    service.info('First');
    service.error('Second');

    expect(service.toasts().map((t) => t.message)).toEqual(['First', 'Second']);
  });

  it('gives each toast a distinct id', () => {
    const a = service.success('A');
    const b = service.success('B');

    expect(a).not.toBe(b);
  });

  it('auto-dismisses after the given duration', fakeAsync(() => {
    service.success('Saved.', 1_000);
    expect(service.toasts().length).toBe(1);

    tick(999);
    expect(service.toasts().length).toBe(1);

    tick(1);
    expect(service.toasts().length).toBe(0);
  }));

  it('keeps errors on screen longer than successes by default', fakeAsync(() => {
    service.success('ok');
    service.error('bad');

    // Default success window elapses first, leaving the error behind to be read.
    tick(5_000);
    expect(service.toasts().map((t) => t.kind)).toEqual(['error']);

    tick(3_000);
    expect(service.toasts().length).toBe(0);
  }));

  it('does not auto-dismiss when the duration is zero', fakeAsync(() => {
    service.error('sticky', 0);

    tick(60_000);
    expect(service.toasts().length).toBe(1);

    service.clear();
  }));

  it('dismisses a specific toast by id', () => {
    const first = service.info('First');
    service.info('Second');

    service.dismiss(first);

    expect(service.toasts().map((t) => t.message)).toEqual(['Second']);
  });

  it('ignores a dismiss for an unknown id', () => {
    service.info('First');

    service.dismiss(9999);

    expect(service.toasts().length).toBe(1);
  });

  it('does not fire a pending timer after a manual dismiss', fakeAsync(() => {
    const id = service.success('Saved.', 1_000);
    service.dismiss(id);

    // Would throw on a double-removal or leave a dangling timer.
    tick(2_000);
    expect(service.toasts().length).toBe(0);
  }));

  it('clear removes everything and cancels timers', fakeAsync(() => {
    service.success('a');
    service.error('b');

    service.clear();
    expect(service.toasts().length).toBe(0);

    tick(10_000);
    expect(service.toasts().length).toBe(0);
  }));
});
